# SerialPacketReceiver
シリアル通信のパケット受信器クラスです。アスキー形式またはバイナリー形式のパケットに対応します。

- アスキー形式：開始コードと終了コードを持つアスキーキャラクタのパケット
- バイナリー形式：開始ヘッダとパケット長指定子を持つバイナリーデータのパケット

## コンストラクタ
|  名前  |  説明  |
| ---- | ---- |
| SerialPacketReceiver(port, maxSize) | port: シリアルポート<br>maxSize: パケットの最大バイト数 |


## イベント
|  名前  |  説明  |
| ---- | ---- |
|  PacketReceived  |  パケットを受信した時  |

## フィールド
|  名前  |  説明  |
| ---- | ---- |
| PollingInterval |  受信ポーリング周期[ミリ秒]  |
| PacketMode | パケットモード。バイナリー形式かアスキー形式か。 |
| TimeOut |  パケットタイムアウト時間[ミリ秒]。<br>パケット先頭バイト受信からこの時間を経過すると受信中のデータは破棄されます。  |
| StartCode |  パケット開始コード (アスキーモード用)  |
| EndCode |  パケット終了コード (アスキーモード用)  |
| Header | パケット開始ヘッダ (バイナリーモード用)  |
| Endian | バイナリーモードのパケットのエンディアン |
| LengthOffset | バイナリーモードのパケット長指定子(Length)の<br>先頭位置 (パケット先頭を0とする) |
| LengthWidth | バイナリーモードのパケット長指定子(Length)の<br>バイト幅 (1または2) |
| LengthExtra | バイナリーモードのパケット長指定子(Length)の追加値。<br>(パケットの全バイト数 - Length)の値を指定する。 |

## メソッド
|  名前  |  説明  |
| ---- | ---- |
|  Start()  |  パケット受信スレッドを開始します。 |
|  Stop()  |  パケット受信スレッドを停止します。<br>フォーム/アプリ終了時までに停止しないとスレッドがゾンビ化します。 |
|  GetAsciiPacket()  |  受信したアスキー形式パケットを取得する (ノンブロッキング)。<br>戻り値: 受信したパケットのバイト配列 |
|  GetBinaryPacket()  |  受信したバイナリー形式パケットを取得する (ノンブロッキング)。<br>戻り値: 受信したパケットのバイト配列 |
| WaitAsciiPacket(timeout) | アスキー形式パケットの受信を待って取得する (ブロッキング)。<br>timeout: タイムアウト時間[ミリ秒]<br>戻り値: 受信したパケットのバイト配列 |
| WaitBinaryPacket(timeout) | アスキー形式パケットの受信を待って取得する (ブロッキング)。<br>timeout: タイムアウト時間[ミリ秒]<br>戻り値: 受信したパケットのバイト配列 |

## 注意点
非同期処理APIと同期処理APIの利用は排他です。

### 非同期処理API
* PacketReceivedイベントにハンドラを登録します。
* Start()で受信スレッドを開始します。
* パケット受信があるとPacketReceivedイベントが発生します。
* GetAsciiPacket()またはGetBinaryPacket()でパケットを取得します。
* フォーム/アプリ終了時までにStop()で受信スレッドを停止します。

### 同期処理API
* WaitAsciiPacket()またはWaitBinaryPacket()でパケット受信を待って取得します。
