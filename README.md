# SerialPortToolBar
シリアル通信ツールバー (Windows Forms) と電文通信のためのクラスライブラリ

![図](README/img.png)

<br>

## クラス

|  名前  |  説明  |
| ---- | ---- |
|  [SerialPortToolStrip](README/SerialPortToolStrip.md)  |  シリアル通信ツールバーのコントロールです。ToolStripクラスを継承しています。  |
|  [SerialPortExtensions](README/SerialPortExtensions.md)  |  SerialPortクラスに拡張メソッドを提供します。  |
|  [SerialPacketReceiver](README/SerialPacketReceiver.md)  |  シリアル通信のパケット受信器  |
|  [AsciiPacket](README/AsciiPacket.md)  |  アスキー形式パケット  |
|  [BinaryPacket](README/BinaryPacket.md)  |  バイナリー形式パケット  |

<br>

## 列挙型

|  名前  |  説明  |
| ---- | ---- |
|  PacketMode  |  アスキー形式かバイナリー形式か  |
|  AsciiCode  |  アスキー制御キャラクタコード  |
|  Endian  |  ビッグエンディアンかリトルエンディアンか  |
