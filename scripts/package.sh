set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet restore ./Aix.UidGenerator.sln
dotnet build ./Aix.UidGenerator.sln -c Release


dotnet pack ./src/Aix.UidGenerator/Aix.UidGenerator.csproj -c Release -o $artifactsFolder
