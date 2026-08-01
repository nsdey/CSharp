param(
    [Parameter(Mandatory = $true)]
    [string]$Path
)

$typeNamespace = 'http://www.w3.org/2001/XMLSchema-instance'
$actionTypes = @{
    ActionAdCloser = 'adCloser'; ActionManualInterrupt = 'manualInterrupt';
    ActionFindHtmlElement = 'findHtmlElement'; ActionMatchTitle = 'matchTitle';
    ActionMatchURL = 'matchUrl'; ActionNavigate = 'navigate'; ActionWait = 'wait'
}
$performTypes = @{
    PerformClick = 'click'; PerformClickOffset = 'clickOffset';
    PerformElementDoesnotExists = 'elementDoesNotExist'; PerformCheckBoxOff = 'checkBoxOff';
    PerformCheckBoxOn = 'checkBoxOn'; PerformCheckBoxToggle = 'checkBoxToggle';
    PerformChooseDropdownItem = 'chooseDropdownItem'; PerformEnterText = 'enterText';
    PerformMatchTitle = 'matchTitle'; PerformMatchUIText = 'matchUiText';
    PerformMatchURL = 'matchUrl'; PerformUploadFile = 'uploadFile'
}

function Convert-Value([System.Xml.XmlElement]$Element) {
    if ($Element.Name -eq 'CheckElementExists') { return [bool]::Parse($Element.InnerText) }
    return $Element.InnerText
}

function Convert-Perform([System.Xml.XmlElement]$Element) {
    $result = [ordered]@{ '$type' = $performTypes[$Element.GetAttribute('type', $typeNamespace)] }
    foreach ($child in $Element.ChildNodes | Where-Object { $_ -is [System.Xml.XmlElement] }) {
        $result[$child.Name] = Convert-Value $child
    }
    return $result
}

function Convert-Action([System.Xml.XmlElement]$Element) {
    $result = [ordered]@{ '$type' = $actionTypes[$Element.GetAttribute('type', $typeNamespace)] }
    foreach ($child in $Element.ChildNodes | Where-Object { $_ -is [System.Xml.XmlElement] }) {
        if ($child.Name -eq 'Perform') { $result.Perform = Convert-Perform $child }
        elseif ($child.Name -notin @('GetAllFindHtmlElementBy', 'GetPerformClassesItem')) { $result[$child.Name] = Convert-Value $child }
    }
    return $result
}

function Convert-Workflow([System.Xml.XmlElement]$Element) {
    $result = [ordered]@{}
    foreach ($child in $Element.ChildNodes | Where-Object { $_ -is [System.Xml.XmlElement] }) {
        if ($child.Name -eq 'Actions') { $result.Actions = @($child.Action | ForEach-Object { Convert-Action $_ }) }
        else { $result[$child.Name] = Convert-Value $child }
    }
    return $result
}

Get-ChildItem -LiteralPath $Path -Filter '*.project' -File | ForEach-Object {
    [xml]$xml = Get-Content -LiteralPath $_.FullName -Raw
    $project = [ordered]@{
        Name = $xml.Project.Name
        BaseURLs = $xml.Project.BaseURLs
        Workflows = @($xml.Project.Workflows.Workflow | ForEach-Object { Convert-Workflow $_ })
        TestCases = @($xml.Project.TestCases.TestCase | ForEach-Object {
            [ordered]@{ Name = $_.Name; WorkflowId_FK = @($_.WorkflowId_FK.string) }
        })
    }
    $jsonFile = [System.IO.Path]::ChangeExtension($_.FullName, '.json')
    $project | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $jsonFile -Encoding utf8
    Write-Output $jsonFile
}
