# Demon Slayer Pets

귀멸의 칼날 캐릭터가 바탕화면을 돌아다니는 Windows 데스크톱 펫입니다. 앱을 켜면 탄지로가 화면 위에서 떨어져 등장합니다. 알림 영역에서 캐릭터를 불러오거나 돌려보내고, 기술을 골라 재생할 수 있습니다.

## 폴더 구조

```text
Demon Slayer/
├── App/
│   ├── DemonSlayerPets.cs       # 앱 진입점, 펫 창, 이동과 기술 재생
│   ├── TrayMenu.cs             # 트레이 메뉴, 캐릭터 소환, 기술 등록
│   ├── PetInput.cs             # 클릭, 드래그, 다음 기술 예약
│   └── PetFall.cs              # 등장·드롭 시 낙하와 착지 효과
├── Characters/
│   ├── Tanjiro/Tanjiro.cs      # 양손 검술, 물의 호흡, 히노카미 카구라
│   ├── Nezuko/Nezuko.cs        # 상자 숨기, 발차기, 분홍 불꽃
│   ├── Zenitsu/Zenitsu.cs      # 벽력일섬과 연속 돌진
│   └── Inosuke/Inosuke.cs      # 톱니 쌍검, 가면 벗기, 별 반짝임
├── Rendering/
│   └── PetArt.cs               # 기본 SD 모습, 눈·도형·번개 그리기
├── assets/
│   └── HinokamiKagura.ico      # 실행 파일과 알림 영역 아이콘
├── build.ps1                   # C# 코드를 Windows 실행 파일로 빌드
└── README.md
```

C# 코드는 `partial class Pet`을 사용합니다. 파일이 여러 폴더에 있어도 빌드할 때 하나의 `Pet` 클래스로 합쳐집니다.

## 어디를 수정하면 되나요?

- **기술을 추가할 때**는 해당 캐릭터 파일에 그리기 함수를 작성하고 [TrayMenu.cs](App/TrayMenu.cs)의 `Skills`에 이름, 함수, 재생 시간을 등록합니다. 목록 순서가 클릭으로 기술이 나오는 순서입니다.
- **캐릭터 생김새를 바꿀 때**는 [PetArt.cs](Rendering/PetArt.cs)의 `DrawCharacter()`부터 봅니다. 이노스케의 맨얼굴과 가면은 [Inosuke.cs](Characters/Inosuke/Inosuke.cs)에 있습니다.
- **조작을 바꿀 때**는 [PetInput.cs](App/PetInput.cs), **낙하나 착지**는 [PetFall.cs](App/PetFall.cs)를 봅니다. 걷는 속도나 자동 기술 간격은 [DemonSlayerPets.cs](App/DemonSlayerPets.cs)에 있습니다.

## Windows에서 빌드하고 실행하기

프로젝트 폴더에서 PowerShell을 열고 아래 명령을 실행합니다. 빌드에는 스크립트가 지정한 Windows의 .NET Framework C# 컴파일러가 필요합니다.

```powershell
.\build.ps1
.\DemonSlayerPets.exe
```

빌드 스크립트는 `App`, `Characters`, `Rendering` 아래의 모든 `.cs` 파일을 찾아 프로젝트 최상위에 `DemonSlayerPets.exe`를 만듭니다. 빌드 후 내장 테스트를 실행하려면 다음 명령을 사용합니다.

```powershell
$check = Start-Process .\DemonSlayerPets.exe -ArgumentList '--check' -Wait -PassThru
$check.ExitCode
```

종료 코드가 `0`이면 테스트를 통과한 것입니다.

## macOS로 이식할 때

현재 소스는 WinForms와 Windows용 `System.Drawing`을 사용합니다. macOS용으로 이식할 때는 `App`의 투명 창·입력·트레이 연결을 macOS의 창과 메뉴 막대에 맞춰 구현하고, `Rendering`과 캐릭터의 그리기 호출도 사용할 그래픽 API에 맞춰 옮겨야 합니다. 캐릭터별 색상, 도형 좌표, 기술 타이밍, 클릭 순환과 낙하 계산은 이식할 때 참고할 수 있습니다.
