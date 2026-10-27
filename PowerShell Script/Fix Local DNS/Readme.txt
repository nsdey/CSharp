How to run it
1. Save the script as Fix-Warp-DNS.ps1.
2. Right-click Start → Terminal (Admin).
3. Run:
Set-ExecutionPolicy -Scope Process Bypass
& "$HOME\Desktop\Fix-Warp-DNS.ps1"


Change the path if you save it somewhere other than your Desktop.

Important: This script assumes your active adapter's gateway also provides DNS, as 192.168.2.1 did during our repair. It does not disable WARP or modify FortiClient settings. If you are connected to your work VPN, disconnect it before running the repair so that its DNS requirements aren't inadvertently affected.