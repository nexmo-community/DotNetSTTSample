using Microsoft.AspNetCore.Http;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TranscriptionBlogPostDemo
{
    public class TranscriptionEngine : IDisposable
    {
        const int SAMPLES_PER_SECOND = 8000;
        const int BITS_PER_SAMPLE = 16;
        const int NUMBER_OF_CHANNELS = 1;
        const int BUFFER_SIZE = 160 * 2;

        SpeechConfig _config = SpeechConfig.FromSubscription("Subscription_key", "region");
        PushAudioInputStream _inputStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(SAMPLES_PER_SECOND, BITS_PER_SAMPLE, NUMBER_OF_CHANNELS));
        AudioConfig _audioInput;
        SpeechRecognizer _recognizer;
        bool _started = false;

        public TranscriptionEngine()
        {
            _audioInput = AudioConfig.FromStreamInput(_inputStream);
        }

        private void RecognizerRecognized(object sender, SpeechRecognitionEventArgs e)
        {
            Trace.WriteLine("Recognized: " + e.Result.Text);
        }

        private async Task StartSpeechTranscriptionEngine(string language)
        {
            _config.SpeechRecognitionLanguage = language;
            _recognizer = new SpeechRecognizer(_config, _audioInput);
            _recognizer.Recognized += RecognizerRecognized;
            await _recognizer.StartContinuousRecognitionAsync();
        }

        private async Task StopTranscriptionEngine()
        {
            if(_recognizer != null)
            {
                _recognizer.Recognized -= RecognizerRecognized;
                await _recognizer.StopContinuousRecognitionAsync();
            }
        }

        public void Dispose()
        {
            _inputStream.Dispose();
            _audioInput.Dispose();
            _recognizer.Dispose();
        }

        public async Task ReceiveAudioOnWebSocket(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[BUFFER_SIZE];

            try
            {
                var language = "en-US";
                await StartSpeechTranscriptionEngine(language);
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    _inputStream.Write(buffer);
                }
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
            finally
            {
                await StopTranscriptionEngine();
            }
        }    
    }
}
