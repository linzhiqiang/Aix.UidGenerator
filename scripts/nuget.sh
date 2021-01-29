set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder


dotnet build ./src/Aix.UidGenerator/Aix.UidGenerator.csproj -c Release

dotnet pack ./src/Aix.UidGenerator/Aix.UidGenerator.csproj -c Release -o $artifactsFolder

dotnet nuget push ./$artifactsFolder/Aix.UidGenerator.*.nupkg -k $PRIVATE_NUGET_KEY -s https://www.nuget.org
