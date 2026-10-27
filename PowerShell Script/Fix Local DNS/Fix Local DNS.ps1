
# Fix-Warp-DNS.ps1
# Repairs DNS after disconnecting Cloudflare WARP.
# Run PowerShell as Administrator.

$ErrorActionPreference = "Continue"

Write-Host "`n=== Cloudflare WARP DNS Repair ===" -ForegroundColor Cyan

# 1. Find the active Ethernet adapter
$adapter = Get-NetAdapter -Name "Ethernet" -ErrorAction SilentlyContinue

if (-not $adapter -or $adapter.Status -ne "Up") {
    Write-Host "Ethernet is not active. Checking other active adapters..." -ForegroundColor Yellow

    $adapter = Get-NetAdapter |
        Where-Object { $_.Status -eq "Up" -and $_.HardwareInterface } |
        Select-Object -First 1
}

if (-not $adapter) {
    Write-Host "No active physical network adapter found. Connect to your network first." -ForegroundColor Red
    exit 1
}

$name = $adapter.Name
Write-Host "Active adapter: $name"

# 2. Verify that the gateway responds
$ipConfig = Get-NetIPConfiguration -InterfaceIndex $adapter.ifIndex
$gateway = $ipConfig.IPv4DefaultGateway.NextHop

if (-not $gateway) {
    Write-Host "No IPv4 gateway found. No changes made." -ForegroundColor Red
    exit 1
}

Write-Host "Gateway: $gateway"

if (-not (Test-Connection -ComputerName $gateway -Count 1 -Quiet)) {
    Write-Host "Gateway is not responding. Check your network/router first." -ForegroundColor Red
    exit 1
}

# 3. Set IPv4 DNS to the gateway (the setting that fixed your laptop)
Write-Host "`nSetting IPv4 DNS to $gateway ..."
Set-DnsClientServerAddress -InterfaceIndex $adapter.ifIndex -ServerAddresses $gateway

# 4. Restore automatic IPv6 DNS to prevent the stale setting recurring
Write-Host "Setting IPv6 DNS to automatic ..."
netsh interface ipv6 set dnsservers name="$name" source=dhcp

# 5. Clear DNS cache
Write-Host "Clearing DNS cache ..."
Clear-DnsClientCache

# 6. Test DNS resolution
Write-Host "`nTesting DNS resolution ..."
try {
    Resolve-DnsName www.google.com -Server $gateway -ErrorAction Stop |
        Select-Object -First 3 Name, Type, IPAddress |
        Format-Table -AutoSize

    Write-Host "DNS lookup succeeded." -ForegroundColor Green
}
catch {
    Write-Host "DNS lookup failed. Check your router DNS settings." -ForegroundColor Red
}

# 7. Test internet connectivity
Write-Host "`nTesting HTTPS connectivity ..."
try {
    $response = Invoke-WebRequest `
        -Uri "https://www.google.com" `
        -Method Head `
        -TimeoutSec 10 `
        -UseBasicParsing

    Write-Host "HTTPS is working. HTTP status: $([int]$response.StatusCode)" -ForegroundColor Green
}
catch {
    Write-Host "HTTPS test failed: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "DNS may be fixed, but further network troubleshooting may be required."
}

Write-Host "`nRepair finished. Test your browser." -ForegroundColor Cyan
