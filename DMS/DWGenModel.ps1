dotnet ef dbcontext scaffold  "data source=192.168.20.200;initial catalog=dw_dms;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;" Microsoft.EntityFrameworkCore.SqlServer -c DWContext  -o DWModels -f --no-build --use-database-names --json
$content = Get-Content -Path 'DWModels\DWContext.cs' -Encoding UTF8
$content = $content -replace "using System;", "using System;using Thinktecture;"
$content = $content -replace "modelBuilder.Entity<ActionDAO>", "modelBuilder.ConfigureTempTable<long>();modelBuilder.Entity<ActionDAO>"
$content | Set-Content -Path "DWModels\DWContext.cs"  -Encoding UTF8