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
| SetRtsControl() | DCB構造体のfRtsControlの値を設定します。(RTS信号の機能を設定します。)<br>必ずポートをOpenしてから設定してください。 |

