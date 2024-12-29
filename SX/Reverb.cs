using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace AudioWorkPlace.SX
{
    public static class Reverb
    {

        public static void ApplyPrimitiveReverb(string inputFile, string outputFile)
        {
            using (var reader = new AudioFileReader(inputFile))
            {
                var outFormat = reader.WaveFormat;
                using (var writer = new WaveFileWriter(outputFile, outFormat))
                {
                    int delayMilliseconds = 500;  // opóźnienie w milisekundach
                    float decay = 0.5f;           // współczynnik zaniku echa
                    int delaySamples = (int)(delayMilliseconds / 1000.0 * reader.WaveFormat.SampleRate) * reader.WaveFormat.Channels;

                    float[] buffer = new float[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
                    float[] delayBuffer = new float[delaySamples];
                    int bufferIndex = 0;

                    int read;
                    while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        for (int i = 0; i < read; i++)
                        {
                            var wetSample = buffer[i] + (bufferIndex < delaySamples ? 0 : delayBuffer[bufferIndex % delaySamples] * decay);
                            delayBuffer[bufferIndex % delaySamples] = buffer[i] + (bufferIndex < delaySamples ? 0 : delayBuffer[bufferIndex % delaySamples] * decay);
                            buffer[i] = wetSample;

                            bufferIndex++;
                        }
                        writer.WriteSamples(buffer, 0, read);
                    }
                }
            }
        }
    }
}
