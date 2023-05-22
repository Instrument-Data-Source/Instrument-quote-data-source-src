project='../src/Configuration/Database/Instrument.Quote.Source.Configuration.DataBase/Instrument.Quote.Source.Configuration.DataBase.csproj'
mig_pr='../src/Configuration/Database/Instrument.Quote.Source.Configuration.DataBase.Migrations.Factory/Instrument.Quote.Source.Configuration.DataBase.Migrations.Factory.csproj'
dotnet ef migrations remove -p $project -s $mig_pr
