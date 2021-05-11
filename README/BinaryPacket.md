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

### Set系
|  名前  |  説明  |
| ---- | ---- |
| SetByte(offset, value)  |  1バイトのデータ(制御コードなど)を格納します。 |
| SetChar(offset, value)  |  1文字のアスキー文字を格納します。 |
| SetString(offset, stringData)  |  文字列を格納します。 |
| SetInt(offset, width, value)  |  整数値を格納します。 |
| SetFloat(offset, value)  |  float型実数値を格納します。 |

### Get系
|  名前  |  説明  |
| ---- | ---- |
| GetByte(offset)  |  1バイトのデータ(制御コードなど)を取得します。 |
| GetChar(offset)  |  1文字のアスキー文字を取得します。 |
| GetString(offset, length)  |  文字列を取得します。 |
| GetInt(offset, width)  |  非負整数値を取得します。 |
| GetIntU(offset, width)  |  符号なし整数値を取得します。 |
| GetIntS(offset, width)  |  符号つき整数値を取得します。 |
| GetFloat(offset)  |   float型実数値を取得します。 |

### チェックサム系
|  名前  |  説明  |
| ---- | ---- |
| Sum(start, length)  |  算術加算によるチェックサム値を計算します。 |
| SetSum(offset, start, length)  |  算術加算によるチェックサム値を計算して指定位置に格納します。 |
| CheckSum(offset, start, length)  |  算術加算によるチェックサム値を計算して指定位置の値と比較します。 |
| Xor(start, length)  |  排他的論理和によるチェックサム値を計算します。 |
| SetXor(offset, start, length)  |  排他的論理和によるチェックサム値を計算して指定位置に格納します。 |
| CheckXor(offset, start, length)  |  排他的論理和によるチェックサム値を計算して指定位置の値と比較します。 |
