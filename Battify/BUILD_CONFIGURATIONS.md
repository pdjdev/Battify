# Battify 빌드 설정 가이드

## 배포 옵션 비교

### 옵션 1: Portable Single File (권장 - 일반 배포용)
**파일 크기**: ~46MB  
**빌드 구성**: Release  
**장점**: 
- 하나의 EXE 파일만 배포
- .NET 런타임 설치 불필요
- 사용자가 바로 실행 가능
- **WinRT API 제외 (23MB 절약)**

**단점**: 파일 크기가 큼 (하지만 Store 빌드보다 23MB 작음)

**설정** (`.csproj` 파일):
```xml
<PublishSingleFile>true</PublishSingleFile>
<SelfContained>true</SelfContained>
<PublishTrimmed>true</PublishTrimmed>
<TrimMode>partial</TrimMode>
<_SuppressWinFormsTrimError>true</_SuppressWinFormsTrimError>
<_SuppressWpfTrimError>true</_SuppressWpfTrimError>
<IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
<IncludeAllContentForSelfExtract>true</IncludeAllContentForSelfExtract>
<EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
<PublishReadyToRun>false</PublishReadyToRun>
<TargetFramework>net8.0-windows</TargetFramework>
<DefineConstants>PORTABLE_BUILD</DefineConstants>
```

**빌드 명령**:
```powershell
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true
```

---

### 옵션 2: Store Build (Microsoft Store 배포용)
**파일 크기**: Battify.dll 14MB + 의존성 DLL들 (Microsoft.Windows.SDK.NET.dll 23MB 포함)  
**빌드 구성**: Store  
**총 크기**: ~38MB

**장점**: 
- MSIX 패키징용 최적화
- WinRT API 사용 가능 (시작프로그램 API)
- Store가 자동으로 최적화

**단점**: 
- 여러 파일 배포
- WinRT SDK 포함으로 크기 증가
- Store 배포 전용

**설정** (`.csproj` 파일):
```xml
<PublishSingleFile>false</PublishSingleFile>
<SelfContained>false</SelfContained>
<PublishTrimmed>false</PublishTrimmed>
<TargetFramework>net8.0-windows10.0.18362.0</TargetFramework>
<TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
<DefineConstants>STORE_BUILD</DefineConstants>
```

**빌드 명령**:
```powershell
dotnet publish -c Store -r win-x86
```

---

### 옵션 3: Framework-Dependent (가장 작음 - 고급 사용자용)
**파일 크기**: ~38MB  
**장점**: 
- Single File로 배포 가능
- 옵션 1보다 작은 크기

**단점**: 
- .NET 8 Desktop Runtime 설치 필요
- 사용자에게 런타임 설치 안내 필요

**설정**:
```xml
<PublishSingleFile>true</PublishSingleFile>
<SelfContained>false</SelfContained>
```

**빌드 명령**:
```powershell
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true
```

**런타임 다운로드**: https://dotnet.microsoft.com/download/dotnet/8.0

---

## 빌드 구성 차이점

### Release (포터블)
- `PORTABLE_BUILD` 정의
- `net8.0-windows` 타겟
- WinRT API 제외
- Registry 방식으로 시작프로그램 설정
- 45MB 단일 파일

### Store (MSIX)
- `STORE_BUILD` 정의
- `net8.0-windows10.0.18362.0` 타겟
- WinRT API 포함 (Microsoft.Windows.SDK.NET.dll 23MB)
- Windows.ApplicationModel.StartupTask API 사용
- DLL 분리 배포

---

## 조건부 컴파일

코드에서 `#if STORE_BUILD`를 사용하여 Store 빌드에서만 WinRT API를 사용합니다:

```csharp
#if STORE_BUILD
using Windows.ApplicationModel;
#endif
```

포터블 빌드에서는 Registry 방식(`StartupSetter`)만 사용하므로 WinRT SDK가 불필요합니다.

---

## 현재 설정 전환 방법

Visual Studio에서:
1. 빌드 구성을 "Release" 또는 "Store"로 선택
2. 빌드 → 게시 실행

명령줄에서:
```powershell
# 포터블 (일반 배포)
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true

# Store (MSIX 패키징)
dotnet publish -c Store -r win-x86
```

---

## 권장 사항

- **일반 사용자 배포**: 옵션 1 (Portable Single File)
- **Microsoft Store 배포**: 옵션 2 (Store Build)
- **고급 사용자**: 옵션 3 (Framework-Dependent)
