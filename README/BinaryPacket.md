# BinaryPacket
バイナリー形式パケットを表します。  
バイナリー形式パケットとは、開始ヘッダとパケット長指定子を持つバイナリーデータのパケットです。

## コンストラクタ
|  名前  |  説明  |
| ---- | ---- |
| BinaryPacket(data, endian) | data: パケットのバイト配列データ<br>endian: エンディアン指定(省略可) |
| BinaryPacket(size, header, endian) | size: パケットの全バイト数(ヘッダ等を含む)<br>header:  ヘッダ<br>endian: エンディアン指定(省略可)|

## フィールド
|  名前  |  説明  |
| ---- | ---- |
| Data |  パケットのバイト配列データ |

## メソッド
|  名前  |  説明  |
| ---- | ---- |
| SetByte(offset, value)  |  1バイトのデータ(制御コードなど)を格納します。 |
| SetChar(offset, value)  |  1文字のアスキー文字を格納します。 |
| SetString(offset, stringData)  |  文字列を格納します。 |
| SetInt(offset, width, value)  |  整数値を格納します。 |
| SetFloat(offset, value)  |  float型実数値を格納します。 |
| byte GetByte(offset)  |  1バイトのデータ(制御コードなど)を取得します。 |
| char GetChar(offset)  |  1文字のアスキー文字を取得します。 |
| string GetString(offset, length)  |  文字列を取得します。 |
| int GetInt(offset, width)  |  非負整数値を取得します。 |
| uint GetIntU(offset, width)  |  符号なし整数値を取得します。 |
| int GetIntS(offset, width)  |  符号つき整数値を取得します。 |
| int GetFloat(offset)  |   float型実数値を取得します。 |
