프로젝트에서 생성한 스크립트, 프리팹, 씬 등 직접 제작한 에셋은 "Assets/\_Project" 하위에서 관리해 주세요.

"Assets/\_Project/Scenes/PrivateTestScene.unity"

"Assets/\_Project/Scenes/PrivateTestScene"

위 두 패스의 씬과 폴더는 깃 이그노어에 등록되어 있어요.

개인적으로 테스트 할 씬이 필요하면 해당 디렉터리로 씬을 생성하셔서 테스트 하시면 되요.



이펙트 프리팹에 AudioSource 컴포넌트를 부착해 이펙트에 소리를 추가하는 경우 유의 사항
Output => Master (MasterMixer)
Volume => .5f (권장 사항)
SpatialBlend => 1f
3DSoundSettings
    VolumeRolloff => LinearRolloff
    MaxDistance => 50f

위의 사양 보다 좋은 셋팅이 있다면 알려주세요



TODO

Refactoring

CurrentStage에 따른 CurrentStageIcon 동기화

적처치시 드랍 시스템 생성

방장이 포톤을 통해 생성한 적이 피해를 받았을때 RPC를 통해 마스터클라이언트 에게 명중에 대한 정보를 전달하도록 변경
방장은 해당 이벤트가 전달 되었다면 해당 메서드를 실행시켜 실제로 피해받도록 변경