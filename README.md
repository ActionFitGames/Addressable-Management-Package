Addressable-Management-Package
===
![GitHub release](https://img.shields.io/github/v/release/ActionFitGames/Addressable-Management-Package.svg)

**ActionFit Organization, Framework-ResourceSystem**
* 효율적인 리소스 관리를 하기 위한 `어드레서블` 활용 시스템
* 철저한 메모리 관리 및 자동화 컴프레션
* 이벤트 핸들러 및 체인 오퍼레이션 제공

<br>

# Table of Contents
* [Feature](#feature)
* [Install via git URL](#install-via-git-url)
* [Getting Started](#getting-started)
* [Basic Usage](#basic-usage)
* [Methods](#methods)
* [Settings](#settings)

<br>

Feature
---
#### 자동 메모리 관리
* `AssetCusntromRef`를 활용하여 `Reference Counting` 기반 자동 메모리 해제
* 사용하지 않는 에셋에 대한 언로드 자동화
* 메모리 누수 방지 시스템 (Memory Tracking)

#### 로딩 최적화
* 비동기 로딩 지원
* 프레임 드랍을 방지하기 위한 순차적 로딩 시스템
* 로딩 트래커를 활용한 프로그레스 트래킹

#### 체인 오퍼레이션
* 비동기 리소스를 동기 메서드에서 활용 가능
* OnComplete, OnError 및 Task 제공

<br>

Install via git URL
---
> Addressable Latest Version : 1.21.21<br>
> Unity Version `22.3.xx` or `greater`

![imgur](https://imgur.com/vCbs1vj.png)

1. Open Unity Project
2. Windows -> Package Manager -> 좌측 `+` 버튼 클릭
3. Package add with Git URL (via)

```
https://github.com/ActionFitGames/Addressable-Management-Package.git
```

위 주소를 복사하여 입력 필드에 삽입 후 패키지 설치를 진행 및 완료

<br>

Getting Started
---
> 리소스 시스템 프리팹(게임 오브젝트) 배치
1. 생성 방법, Unity Tool Bar

![imgur](https://imgur.com/yqteSId.png)

2. 생성 방법, Hierarchy - Right Click

![imgur](https://imgur.com/lPbrNIY.png)

올바르게 리소스 시스템이 하이어라키에 배치가 되었다면 보다 쉽게 셋팅에 접근할 수 있습니다.

![imgur](https://imgur.com/Aq0rUgB.png)

<br>

Basic Usage
---
> 리소스에 접근할 수 있는 키로는 기존 어드레서블과 동일합니다.

* `Primary Key`
  * 문자열 (String) 어드레서블 그룹에 존재하는 엔트리의 이름
* `AssetReference`
  * 어드레서블에서 제공해주는 Guid 레퍼런스

#### 리소스를 사용 하는 방법 (접근 엑세스)

```csharp
public class ResourceUsableBehavior : MonoBehaviour
// 모노를 상속 받는 비해비어 컴포넌트 (일반적으로 사용 됌)
// 컴포넌트 형식으로 제공 사용할 곳에 상속받으면 됌

public class ResourceUsable : IDisposable
// 일반적인 클래스로 모노가 아니지만 리소스를 불러올 일이 있을 때
```

```csharp
public class MyComponent : ResourceUsableBehavior
{
  public AssetReference SpriteRef;
  public Image MyButtonIcon;

  public void Start()
  {
    GetAsset<Sprite>(SpriteRef).OnComplete((loadedSprite) =>
    {
      MyButtonIcon.sprite = loadedSprite;
    }.OnError(() => Debug.LogError("Can't load"));
  }
}
```

일반적인 예시는 위 코드와 같습니다. 상속을 받으면 GetAsset에 접근할 수 있습니다.

<br>

Methods
---

#### ResourceUsableBehavior & ResourceUsable

```csharp
protected IProvideLoadOperation<T> GetAsset<T>(object assetKeyOrigin) where T : Object
```
* 비동기 오퍼레이션을 동기에서 사용할 수 있게끔 체인 오퍼레이션 핸들 제공
  * `OnComplete(T result)`
  * `OnError(Exception exception)`
* Parameter
  * AssetReference, string (PrimaryKey) 두 가지 파라미터만 허용
* Dyanamic Support
  * 동적 로드를 허용하며, 만약 사전에 로드 되지 않았을 경우 로드 및 캐싱 이후 에셋 반환
* `GameObject Type`은 사용이 거부 됌 (Instantiate)를 사용해야만 함
 
```csharp
protected bool TryGetAsset<T>(object assetKeyOrigin, out T result) where T : Object
```
* 동기식 메서드로 `PrepareLoader`를 통해 사전 로드가 되어 있는 에셋을 가져옴
  * 만약 로드가 안되어 있을 경우 `false`를 반환
  * `GetAsset FetchLodaer`(동적)로 불러온 에셋이 살아 있다고 가정할 경우 에셋 가져오기 가능
* Safety 메서드로 캐싱된 에셋에만 접근하며 만약 캐싱이 안된 에셋은 불러오지 못함

```csharp
protected IProvideInstantiateOperation Instantiate(object assetKeyOrigin, Transform parent = null)
protected IProvideInstantiateOperation Instantiate(object assetKeyOrigin, Vector3 position, Quaternion rotation, Transform parent = null)
```
* 비동기 오퍼레이션을 동기에서 사용할 수 있게끔 체인 오퍼레이션 핸들 제공
  * `OnComplete(GameObject result)`
  * `OnError(Exception exception)`
* 동기식 즉, Safety 메서드는 제공되지 않으며 오로지 체인으로만 제공

```csharp
protected void ReleaseManually(AssetReference assetKeyOrigin)
protected void ReleaseManually(string primaryKey)
```
* 수동 메모리 해제 메서드 (거의 쓸 일이 없음)
* 그래도 수동이 필요할 경우 해당 컴포넌트 또는 클래스에서 로드한 리소스만을 기준으로 해제 가능
  * 타 컴포넌트에 접근해서 키를 가져와 해제하려고 할 시 해제 못함

#### Resource (Global Access)


```csharp
public static IPrepareLoadOperation LoadPrepare(object labelReference)
```
* 비동기 오퍼레이션을 동기에서 사용할 수 있게끔 체인 오퍼레이션 핸들 제공
  * `OnComplete()`
  * `OnError(Exception exception)`
  * `OnProgress(float progress)`
    * 프로그레스는 현재 진행 상황 로딩 트래커로부터 실제 퍼센트 컴플리트 추적을 함
* Key 값으로는 `List<AssetLabelReference>` or `AssetLabelReference`만 허용 됌

```csharp
public static void ReleasePrepare(object labelReferenceOrigin)
```
* Prepare를 통해 레이블 단위로 사전로드(Preload) 했던 에셋들을 해제
* Key 값은 LoadPrepare와 동일
* Preload 했던 에셋들은 직접 해제 하지 않으면 해제 되지 않기에 **씬이 언로드되고 나서 진행 하는 것이 일반적**

<br>

Settings
---
> 셋팅에 진입 하는 방법은 총 `2`가지가 존재합니다.
>  1. ToolBar에서 들어가는 방법
>  2. 프리팹에 있는 컴포넌트에서 들어가는 방법

![imgur](https://imgur.com/SvhO3JA.png)

![imgur](https://imgur.com/ginuirl.png)
* `DebugMode`
  * 어드레서블 전용 디버깅 토글
  * Define Symbol로 적용이 되어 있음
* `DontDestroyOnLoad`
  * Persistent 같은 씬이 존재하지 않을 경우에 사용
  * Enable이 되어 있을 경우 이름과 같이 영구적인 씬으로 넘어감
* `Batch Release Threshold`
  * 릴리즈 할 에셋 한계치로 이를 넘어가야만 실제 릴리즈를 진행
  * 5개 설정 시 릴리즈 가능 한 에셋이 4개면 릴리즈를 진행하지 않음
* `Memory Cleanup Interval`
  * 릴리즈를 간격 시간 설정
  * 무분별한 릴리즈를 막기 위해 해당 쿨타임이 지나야만 릴리즈가 진행 됌



