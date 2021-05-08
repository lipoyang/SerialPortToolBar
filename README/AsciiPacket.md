# AsciiPacket
アスキー形式パケットを表します。  
アスキー形式パケットとは、開始コードと終了コードを持つアスキーキャラクタのパケットです。

## コンストラクタ
|  名前  |  説明  |
| ---- | ---- |
| AsciiPacket(data) | data: パケットのバイト配列データ |
| AsciiPacket(size, startCode, endCode) | size: パケットの全バイト数(開始コード/終了コードを含む)<br>startCode:  開始コード(省略可)<br>endCode: 終了コード(省略可)|

## フィールド
|  名前  |  説明  |
| ---- | ---- |
| Data |  パケットのバイト配列データ |

## メソッド
|  名前  |  説明  |
| ---- | ---- |
| ToString() | パケットデータを文字列に変換します。 |
| SetByte(offset, value)  |  1バイトのデータ(制御コードなど)を格納します。 |
| SetChar(offset, value)  |  1文字のアスキー文字を格納します。 |
| SetString(offset, stringData)  |  文字列を格納します。 |
| SetHex(offset, width, value)  |  整数値を16進文字列に変換して格納します。 |
| SetDec(offset, width, value)  |  非負整数値を10進文字列に変換して格納します。 |
| byte GetByte(offset)  |  1バイトのデータ(制御コードなど)を取得します。 |
| char GetChar(offset)  |  1文字のアスキー文字を取得します。 |
| string GetString(offset, length)  |  文字列を取得します。 |
| GetHex(offset, width, out int value)  |  16進文字列を非負整数値に変換して取得します。 |
| GetDec(offset, width, out int value)  |  10進文字列を非負整数値に変換して取得します。 |
| GetHexU(offset, width, out uint value)  |  16進文字列を符号なし整数値に変換して取得します。 |
| GetHexS(offset, width, out int value)  |  16進文字列を符号つき整数値に変換して取得します。 |
