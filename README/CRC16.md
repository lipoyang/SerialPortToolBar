# CRC16
CRC-16を計算します。

## コンストラクタ
|  名前  |  説明  |
| ---- | ---- |
| CRC16(poly, init, shift, xorout) | poly: 生成多項式値<br>init: 初期値<br>shift: シフト方向<br>xorout: 出力XOR値 |

## メソッド
|  名前  |  説明  |
| ---- | ---- |
| Get(data, offset, length)  |  CRC-16を計算します。<br>data: バイトデータ配列<br>offset: 開始位置<br>length: 長さ(バイト数) |

## 定数

以下の定数は生成多項式を表します。

|  名前  |  値  | 生成多項式 | シフト方向 |
| ---- | ---- | ---- |---- |
| CRC16Poly.IBM_Left  | 0x8005 | x16 + x15 + x2 + 1 | 左 |
| CRC16Poly.IBM_Right  | 0xA001 | x16 + x15 + x2 + 1 | 右 |
| CRC16Poly.CCITT_Left  | 0x1021 | x16 + x12 + x5 + 1 | 左 |
| CRC16Poly.CCITT_Right  | 0x8408 | x16 + x12 + x5 + 1 | 右 |

