<p align="center"><img src="img/red.png"></p>

# LUISBankingBot

This bot uses LUIS (Language understanding intelligence service). Check out this [Overview of the LUIS service]. You might also need some [Cognitive Services Keys]. This app uses some prebuilt composite entities (like number and money) for ease of implementation, a composite entity is just a bunch o' entities mashed together.

[Publish your Bot Application to Microsoft Azure] and show it to the world.

You can also [test your bot in a webapp]. The app is contained within an iframe of a webpage. In the iframe, just include your app secret generated after properly configuring your bot endpoint in the tutorial when you [Publish your Bot Application to Microsoft Azure]. This webapp is just an instance of your bot, the same way these instances appear in Facebook Messenger, Slack, GroupMe etc.

If you wish to continue to test locally using the Microsoft Bot Channel Emulator, keep these [common error codes] in mind when running the bot locally in the emulator.

As of 8/4/2016 LUIS has the following limits on call usage.
| Plan | Description | Price|
|---	|---	|---	|
| Free | 10,000 transactions per month | Free |
| Standard | 10 transactions per second | $0.75 per 1000 transactions |

For fun I added emoji to the bot dialog. All emoji can be found on the [Emojipedia].

Test out the [bing speech to text and text to speech services] too if you want. [Docs for bot with Bing speech api].

[bing speech to text and text to speech services]: <https://www.microsoft.com/cognitive-services/en-us/speech-api>
[Cognitive Services Keys]: <https://www.microsoft.com/cognitive-services/en-us/sign-up>
[common error codes]: <https://blogs.msdn.microsoft.com/benjaminperkins/2016/08/01/bot-framework-405-method-not-allowed-401-unauthorized-and-500-internal-server-error-getting-started/>
[Docs for bot with Bing speech api]: <https://docs.botframework.com/en-us/bot-intelligence/speech/#example-speech-to-text-bot>
[Emojipedia]: <http://emojipedia.org/>
[Overview of the LUIS service]: <https://www.luis.ai/Help>
[Publish your Bot Application to Microsoft Azure]: <https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#publishing>
[test your bot in a webapp]: <https://docs.botframework.com/en-us/support/embed-chat-control2/>
