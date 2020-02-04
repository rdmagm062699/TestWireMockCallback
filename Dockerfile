FROM mcr.microsoft.com/dotnet/core/sdk:3.1

COPY ./AcceptanceTests/ AcceptanceTests/
COPY ./src/ src/
COPY ./run_tests.sh .

RUN rm -rf ./AcceptanceTests/bin ./AcceptanceTests/obj
RUN rm -rf ./src/bin ./src/obj

RUN dotnet build ./AcceptanceTests/AcceptanceTests.csproj
RUN dotnet build ./src/TestWireMockCallback.csproj

ENTRYPOINT [ "/bin/bash", "run_tests.sh" ]
