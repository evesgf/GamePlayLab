using UnityEditor;

namespace Slate {

    [InitializeOnLoad]
	public static class ApplyFrameCapturerDefine {
		static ApplyFrameCapturerDefine(){
			if (!DefinesManager.HasDefine(Prefs.USE_FRAMECAPTURER_DEFINE)){
				DefinesManager.SetDefineActive(Prefs.USE_FRAMECAPTURER_DEFINE, true);
			}
		}
	}
}