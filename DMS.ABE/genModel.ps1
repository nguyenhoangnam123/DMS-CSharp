﻿dotnet ef dbcontext scaffold  "data source=192.168.20.200;initial catalog=dms;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;" Microsoft.EntityFrameworkCore.SqlServer -c DataContext  -o Models -f --no-build --use-database-names --json --schema MDM --schema PER --schema dbo --schema ENUM --schema PRO
$content = Get-Content -Path 'Models\DataContext.cs' -Encoding UTF8
$content = $content -replace "using System;", "using System;using Thinktecture;"
$content = $content -replace "modelBuilder.Entity<ActionDAO>", "modelBuilder.ConfigureTempTable<long>();modelBuilder.Entity<ActionDAO>"
$content | Set-Content -Path "Models\DataContext.cs"  -Encoding UTF8