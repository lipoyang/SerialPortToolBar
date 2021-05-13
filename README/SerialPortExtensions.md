# SerialPortExtensions
SerialPortクラスに拡張メソッドを提供します。

## メソッド
|  名前  |  説明  |
| ---- | ---- |
| WriteBytes(byte[] data) | バイト配列を送信します。 |
| WriteChars(char[] data) | キャラクタ配列を送信します。 |
| WriteByte(byte data) | バイトデータを送信します。 |
| WriteChar(char data) | キャラクタデータを送信します。 |
| byte[] ReadBytes() | 受信データをバイト配列として取得します。 |
| char[] ReadChars() | 受信データをキャラクタ配列として取得します。 |
| SendPacket(packet) | アスキー形式パケット([AsciiPacket](AsciiPacket.md))またはバイナリー形式パケット([BinaryPacket](BinaryPacket.md))を送信します。 |
| SetHalfDuplex() | 半二重通信に設定します。 (RS-485用)<br>必ずポートをOpenしてから設定してください。 |
| ClearHalfDuplex() | 半二重通信の設定を解除します。 (RS-485用)<br>必ずポートをCloseする前に実行してください。 |

