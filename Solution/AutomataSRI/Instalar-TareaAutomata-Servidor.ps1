# Ejecutar como Administrador en el servidor
# PowerShell -ExecutionPolicy Bypass -File "Instalar-TareaAutomata-Servidor.ps1"

$ExePath  = "C:\SICE\apps\Automata\AutomataSRI.exe"
$TaskName = "AutomataSRI"

if (-not (Test-Path $ExePath)) {
    Write-Error "No se encontro el ejecutable: $ExePath"
    exit 1
}

$existing = Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue
if ($existing) {
    Write-Host "Eliminando tarea existente '$TaskName'..."
    Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
}

$action   = New-ScheduledTaskAction -Execute $ExePath -WorkingDirectory "C:\SICE\apps\Automata"
# Ejecuta cada hora desde las 07:00, durante 17 horas (hasta las 00:00)
$trigger  = New-ScheduledTaskTrigger -RepetitionInterval (New-TimeSpan -Hours 1) -RepetitionDuration (New-TimeSpan -Hours 17) -Once -At "07:00"
$settings = New-ScheduledTaskSettingsSet `
    -MultipleInstances IgnoreNew `
    -ExecutionTimeLimit (New-TimeSpan -Minutes 30) `
    -StartWhenAvailable

Register-ScheduledTask `
    -TaskName  $TaskName `
    -Action    $action `
    -Trigger   $trigger `
    -Settings  $settings `
    -RunLevel  Highest `
    -User      "SYSTEM" `
    -Force | Out-Null

Write-Host "Tarea '$TaskName' instalada correctamente."
Write-Host "  Ejecutable : $ExePath"
Write-Host "  Intervalo  : cada 60 minutos de 07:00 a 00:00"
Write-Host "  Usuario    : SYSTEM"
Write-Host "  Instancias : IgnoreNew (no lanza si ya esta corriendo)"
