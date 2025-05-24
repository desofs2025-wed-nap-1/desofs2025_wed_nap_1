# DESOFS2025_WED_NAP_1 - Phase 2 - Sprint 1

This `README` file contains all information about developments that were done during the 1st Sprint of the 2nd Phase of the DESOFS course unit Project.

## Development

### Authentication

### Developed Use Cases

### Coding best practices

(documentar processo de code reviews, branching model - ou seja, ter branches de dev e só vai para main com um Pull Request que passe todos os testes e que seja revisto por pelo menos uma pessoa da equipa)

### Static Code Analysis


To ensure our code quality, maintainability, and security, we integrated **SonarCloud** into our development pipeline. SonarCloud automatically analyzes our C# codebase on each push to the `main` branch using a GitHub Actions workflow.

The workflow performs the following steps:

- **Checkout code and setup environment**

```yaml
- name: Checkout code
  uses: actions/checkout@v4

- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '8.0.x'  
```

- **Install SonarScanner tool and configure PATH**

```yaml
- name: Install dotnet-sonarscanner
  run: dotnet tool install --global dotnet-sonarscanner

- name: Add dotnet tools to PATH
  run: echo "$HOME/.dotnet/tools" >> $GITHUB_PATH
```

- **Begin SonarCloud analysis**

```yaml
- name: Begin SonarCloud analysis
  working-directory: ParkingSystem/ParkingSystem
  run: >
    dotnet-sonarscanner begin
    /k:"JoanaGMoreira_desofs2025_wed_nap_1"
    /o:"joanagmoreira"
    /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
    /d:sonar.host.url="https://sonarcloud.io"
```

- **Restore dependencies and build solution**

```yaml
- name: Restore dependencies
  working-directory: ParkingSystem/ParkingSystem
  run: dotnet restore ParkingSystem.sln

- name: Build solution
  working-directory: ParkingSystem/ParkingSystem
  run: dotnet build ParkingSystem.sln --no-restore --no-incremental
```

- **End SonarCloud analysis**

```yaml
- name: End SonarCloud analysis
  working-directory: ParkingSystem/ParkingSystem
  run: dotnet-sonarscanner end /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
```

SonarCloud provides continuous feedback on:
- Code smells
- Bugs
- Security vulnerabilities
- Test coverage

All team members are encouraged to regularly check SonarCloud reports and fix identified issues as part of the development process.

Below is an example of a SonarCloud report generated during this sprint:

![SonarCloud Dashboard Screenshot](./img/SonarCloud.png)


### Software Composition Analysis

(falar da SBOM, a tool usada para a gerar, e respetivo worfklow. mostrar a SBOM nesta secção do relatório para termos evidências)

## Build and Test

This section will describe the build and test procedure for the application, highlighting relevant procedures.

### Build process

To build our application in a repeatable and efficient way, we chose to adopt Docker - thus, our main artifact is a Docker image. This image is built based on our [`Dockerfile`](../../../ParkingSystem/Dockerfile), which itself uses two Microsoft images (with our project being developed in C# and using the .NET framework):

1. Firstly, `dotnet/sdk:8.0` is used to build the application using the necessary `dotnet publish` command, which generates the applicational artifacts in the `out/` directory.
2. The `aspnet:8.0` image is used to actually execute the application, using the ASP.NET framework. Here, `ParkingSystem.dll` is extracted from the build process and executed via the `ENTRYPOINT` instruction.

This process is not manual, but rather done in a fully automatic way via the usage of GitHub workflows. Notably, the [`build_api.yaml`](../../../.github/workflows/build_api.yaml) is responsible for doing this whenever a pull request to the main branch is created.

The workflow logs in Docker Hub, using a username and password which are stored in the repository as **GitHub secrets** - thus, not being exposed in clear-text anywhere in the repository's code. Then, it uses the `docker/build-push-action@v6` action available in the Marketplace to build the above-mentioned Dockerfile, tag it with the `lew6s/parking-system:0.0.1`, and push it to Docker Hub. The app version is set as `0.0.1` for the time being, and is sourced from an environment variable specified in the workflow.

The mentioned workflow contains other actions for distinct purposes, which will be explained further in this report.

### Execution of test plans

(falar dos testes aplicacionais que temos e de como são executados - por meio de workflow)

### Artifact Scanning

After the build process mentioned prior, we conduct artifact scanning to identify vulnerabilities in the generated artifact. For this use case, we adopted the usage of **Trivy**, which is able to scan Docker images and report on findings. Like other processes, this is fully automated via a GitHub workflow called [`build_api.yaml`](../../../.github/workflows/build_api.yaml). 

In it, the `aquasecurity/trivy-action@0.28.0` GitHub action is used to scan the Docker image artifact after it has been published to Docker Hub:

```YAML
- name: Scan built Docker image with Trivy
  uses: aquasecurity/trivy-action@0.28.0
  with:
    image-ref: 'lew6s/parking-system:${{ env.VERSION }}'
    format: 'sarif'
    output: 'trivy-results.sarif'
    exit-code: '0'
    ignore-unfixed: true
    vuln-type: 'os,library'
    scanners: 'vuln,secret,misconfig'
    severity: 'MEDIUM,HIGH,CRITICAL'
```

This job conducts the scan and generates an output file called `trivy-results.sarif`, which contains a report of the vulnerabilities that were found. These findings are then published to the GitHub security tab via the following job step:

```YAML
- name: Upload Trivy scan results to GitHub Security tab
  uses: github/codeql-action/upload-sarif@v3
  with:
    sarif_file: 'trivy-results.sarif'
```

Below, we can see an example of the findings reported by Trivy:

![trivyScanSecurity.png](./img/trivyScanSecurity.png)

### Dynamic Analysis

After conducting the artifact scan, we run a dynamic analysis of the application. For this, we used the `OWASP ZAP` tool. Notably, we used the `zaproxy/action-api-scan@v0.9.0` GitHub action, which specifically conducts a scan for Web APIs - unlike other OWASP ZAP scans. These tests are fully automated through the [`build_api.yaml`](../../../.github/workflows/build_api.yaml) workflow.

Since we currently don't have a production deployment of the application, the aforementioned Docker image is executed as a service container for this job, making it so that it's accessible by the job itself on the `localhost` of the runner:

```YAML
services:
  api:
    image: 'lew6s/parking-system:0.0.1'
    ports:
      - 8080:8080
```

Then, we have a step which runs the ZAP API scan, targeting the application's container:

```YAML
steps:
  - name: Run ZAP test targeting API
    uses: zaproxy/action-api-scan@v0.9.0
    with:
      target: 'http://localhost:8080/'
```

This action generated a report, which is set as an artifact on the build itself:

![zapScanArtifact.png](./img/zapScanArtifact.png)

However, for convenience and further analysis of the findings, it also opens a **GitHub issue** which reports the findings, so that the team can look into and fix them:

![zapScanIssue.png](./img/zapScanIssue.png)

If this issue is not resolved, it's updated with new findings (if that's the case) whenever the job executes.

### Configuration Validation

(falar de tools que temos para detetar secrets a serem pushados para o repo - temos evidências disto, é só explicar e colar)

## Pipeline Automation

(isto meio que já foi falado nos restantes pontos, portanto acho que é brevemente repetir que todos estes processos estão automatizados por meio de GitHub workflows)

## ASVS Checklist

(colar resultados gerados)

