#!/bin/sh
set -e

echo "Deploying schema via sqlpackage..."
sqlpackage /Action:Publish \
  /SourceFile:/app/CatsDb.dacpac \
  /TargetConnectionString:"$ConnectionStrings__CatsDb" \
  /p:BlockOnPossibleDataLoss=false

echo "Schema deployed successfully."
