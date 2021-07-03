/*using UnityEngine;
using UnityEditor;  // Most of the utilities we are going to use are contained in the UnityEditor namespace

// We inherit from the AssetPostProcessor class which contains all the exposed variables and event triggers for asset importing pipeline
using System.Runtime.InteropServices;
using UnityEngine.Events;


internal sealed class CustomAssetImporter : AssetPostprocessor {

	#region Methods

	//-------------Pre Processors
	// This event is raised every time an audio asset is imported
	private void OnPreprocessAudio() {

		//Get the reference to the assetImporter (From the AssetPostProcessor class) and unbox it to an AudioImporter (wich is inherited and extends the AssetImporter with AudioClip-specific utilities)
		var importer = assetImporter as AudioImporter;
		//if the variable is empty, "do nothing"
		if (importer == null) return;

		//create a temp variable that contains everything you want to apply to the imported AudioClip (possible changes: .compressionFormat, .conversionMode, .loadType, .quality, .sampleRateOverride, .sampleRateSetting)
		AudioImporterSampleSettings sampleSettings = importer.defaultSampleSettings; 
		sampleSettings.loadType = AudioClipLoadType.Streaming; //alternatives: .DecompressOnLoad, .Streaming
		sampleSettings.compressionFormat = AudioCompressionFormat.MP3; //alternatives: .AAC, .ADPCM, .GDADPCM, .HEVAG, .MP3, .PCM, .VAG, .XMA
		sampleSettings.quality = 1f; //ranging from 0 (0%) to 1 (100%), currently set to 1%, wich is the smallest value that can be set in the inspector | Probably only useful when the compression format is set to Vorbis

		importer.defaultSampleSettings = sampleSettings; //applying the temp variable values to the default settings (most important step!)
		//platform-specific alternative:
		//importer.SetOverrideSampleSettings ("Android", sampleSettings); //platform options: "Webplayer", "Standalone", "iOS", "Android", "WebGL", "PS4", "PSP2", "XBoxOne", "Samsung TV"


	}

	//-------------Post Processors

	// This event is called as soon as the audio asset is imported successfully
	//private void OnPostprocessAudio(AudioClip import) {}

	#endregion
}*/