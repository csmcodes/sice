# Instala la tarea programada del Automata SRI en Windows Task Scheduler
# Ejecutar como Administrador

param(
    [string]$ExePath    = "$PSScriptRoot\bin\Release\AutomataSRI.exe",
    [string]$TaskName   = "AutomataSRI",
    [string]$TaskUser   = "SYSTEM",
    [int]   $IntervalMinutes = 60
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $ExePath)) {
    Write-Error "No se encontro el ejecutable: $ExePath`nCompila el proyecto en Release antes de instalar."
    exit 1
}

# Eliminar tarea existente si la hay
$existing = Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue
if ($existing) {
    Write-Host "Eliminando tarea existente '$TaskName'..."
    Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
}

$action  = New-ScheduledTaskAction -Execute $ExePath -WorkingDirectory (Split-Path $ExePath)
$trigger = New-ScheduledTaskTrigger -RepetitionInterval (New-TimeSpan -Minutes $IntervalMinutes) -Once -At (Get-Date).Date
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
    -User      $TaskUser `
    -Force | Out-Null

Write-Host "Tarea '$TaskName' instalada correctamente."
Write-Host "  Ejecutable : $ExePath"
Write-Host "  Intervalo  : cada $IntervalMinutes minutos"
Write-Host "  Usuario    : $TaskUser"
Write-Host "  Instancias : IgnoreNew (no lanza si ya esta corriendo)"
