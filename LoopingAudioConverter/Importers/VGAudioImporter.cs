﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VGAudio.Containers.Adx;
using VGAudio.Containers.Dsp;
using VGAudio.Containers.Genh;
using VGAudio.Containers.Hca;
using VGAudio.Containers.Hps;
using VGAudio.Containers.Idsp;
using VGAudio.Containers.NintendoWare;
using VGAudio.Formats;

namespace LoopingAudioConverter {
	public class VGAudioImporter : IAudioImporter {
		public bool SupportsExtension(string extension) {
			if (extension.StartsWith("."))
				extension = extension.Substring(1);
			switch (extension) {
				case "adx":
				case "brstm":
				case "bcstm":
				case "bcstp":
				case "bcwav":
				case "cwav":
				case "bfstm":
				case "bfstp":
				case "bfwav":
				case "brwav":
				case "rwav":
				case "dsp":
				case "mdsp":
				case "idsp":
				case "genh":
				case "hca":
				case "hps":
					return true;
				default:
					return false;
			}
		}

		private static AudioData Read(byte[] data, string filename) {
			string extension = Path.GetExtension(filename).ToLowerInvariant();
			if (extension.StartsWith("."))
				extension = extension.Substring(1);
			switch (extension) {
				case "adx":
					return new AdxReader().Read(data);
				case "brstm":
					return new BrstmReader().Read(data);
				case "bcstm":
				case "bcstp":
				case "bcwav":
				case "cwav":
				case "bfstm":
				case "bfstp":
				case "bfwav":
					return new BCFstmReader().Read(data);
				case "brwav":
				case "rwav":
					return new BrwavReader().Read(data);
				case "dsp":
				case "mdsp":
					return new DspReader().Read(data);
				case "idsp":
					return new IdspReader().Read(data);
				case "genh":
					return new GenhReader().Read(data);
				case "hca":
					return new HcaReader().Read(data);
				case "hps":
					return new HpsReader().Read(data);
				default:
					throw new NotImplementedException();
			}
		}

		public async Task<PCM16Audio> ReadFileAsync(string filename) {
			if (filename.Contains('"')) {
				throw new AudioImporterException("File paths with double quote marks (\") are not supported");
			}

			byte[] data = File.ReadAllBytes(filename);
			if (data.Length == 0) {
				throw new AudioImporterException("Empty input file");
			}

			try {
				await Task.Yield();
				return PCM16Factory.FromAudioData(Read(data, filename));
			} catch (Exception e) {
				throw new AudioImporterException("Could not convert from B" + (char)data[0] + "STM: " + e.Message);
			}
		}

		public PCM16Audio ReadFile(string filename) {
			if (filename.Contains('"')) {
				throw new AudioImporterException("File paths with double quote marks (\") are not supported");
			}

			byte[] data = File.ReadAllBytes(filename);
			if (data.Length == 0) {
				throw new AudioImporterException("Empty input file");
			}

			try {
				return PCM16Factory.FromAudioData(Read(data, filename));
			} catch (Exception e) {
				throw new AudioImporterException("Could not convert from B" + (char)data[0] + "STM: " + e.Message);
			}
		}
	}
}
