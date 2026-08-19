# stable-to-lazer importer

osu!stableに取り込まれたmapsetを、osu!lazerに同時にハードリンクするCLIツールです。lazer の beatmap parser、Realm model、file store を再利用し、取り込み時には NTFS hard link を要求します。

> これは非公式プロジェクトであり、ppy Pty Ltd, osu! に認められたものではありません。

## ダウンロード

`THIRD_PARTY_NOTICES.md`に記載があるように、
ライセンス上の問題からソースのみの公開となっています。

各自でビルドを行ってください。

## ビルド

```powershell
dotnet build -c Release -warnaserror
```

依存する `ppy.osu.Game*` のバージョンは意図的に固定されています。
実ユーザーデータで使う前に、現行のosu!lazerとの互換性を確認してください。

## 使い方

osu!lazer を完全に終了してから開始してください。

```powershell
.\osu-stable-to-lazer.exe "C:\Users\<username>\AppData\Local\osu!\Songs\123 Artist - Title"
```

lazerのインストール位置を変更している場合は、明示的に指定することができます。

```powershell
.\osu-stable-to-lazer.exe "C:\Users\<username>\AppData\Local\osu!\Songs\123 Artist - Title" --lazer-data "C:\Users\<username>\AppData\Roaming\osu"
```

### 新規 beatmap set の自動監視

`watch.ps1` は stable の `Songs` フォルダ直下で新規ディレクトリが作成されたことを検知し、5 秒待機してからそのフォルダを importer へ渡します。既存のmapsetの走査は行いません。

1. `dotnet build -c Release -warnaserror` を実行します。
2. `watch.ps1` の `$SongsPath` を自分の stable `Songs` フォルダへ変更します。
3. lazer を完全に終了した状態で`watch.ps1`を、ビルドした`osu-stable-to-lazer.exe`と同一のディレクトリに配置し、実行します。

```powershell
.\watch.ps1
```

停止するには `Ctrl+C` を押します。

osu!lazerの起動中はツールの実行ができません。

lazerが起動中の場合、stableの書き込みが5秒以内に完了しない場合、importに失敗した場合などには、ツールは自動再試行しません。

終了コードを確認して、対象フォルダを手動で再実行してください。

stableのSongsフォルダと、lazerのデータフォルダは同じ NTFS ボリューム上に置く必要があります。

## ライセンスと商標

このプロジェクトの source code は [MIT License](LICENSE) で提供します。
この licence は third-party package や asset の利用権を与えるものではありません。
osu!lazer の帰属表示と依存関係の義務については [第三者通知](THIRD_PARTY_NOTICES.md) を参照してください。`osu!`、`osu`、`lazer`、ppy の branding は ppy Pty Ltd に帰属し、このリポジトリは公式プロジェクトではありません。
