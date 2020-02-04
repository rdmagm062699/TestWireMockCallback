#!/usr/bin/env bash

set -e

dotnet test --no-build --no-restore --verbosity normal ./AcceptanceTests/AcceptanceTests.csproj
