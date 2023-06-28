dotnet tool install --global dotnet-ef
echo 'export PATH="$PATH:/home/vscode/.dotnet/tools"' >> ~/.zshrc
sudo apt-get update
sudo apt-get install -y postgresql-client
psql --version
dotnet dev-certs https
dotnet build "/workspaces/Instrument-quote-data-source-srv/Instrument.Quote.Source.Srv.sln"