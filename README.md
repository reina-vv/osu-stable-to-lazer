# stable-to-lazer importer

osu!stable の単一 beatmap set フォルダを、既存の osu!lazer データへ取り込む Windows コマンドラインツールです。lazer の beatmap parser、Realm model、file store を再利用し、取り込み時には NTFS hard link を要求します。

> 非公式プロジェクトです。ppy Pty Ltd または osu! とは提携・承認・サポート関係にありません。

## 使い方

osu!lazer を完全に終了し、データフォルダをバックアップしてから実行してください。

```powershell
.\osu-stable-to-lazer.exe "C:\Users\<username>\AppData\Local\osu!\Songs\123 Artist - Title"
```

自動検出される lazer データフォルダが適切でない場合は、明示的に指定します。

```powershell
.\osu-stable-to-lazer.exe "C:\Users\<username>\AppData\Local\osu!\Songs\123 Artist - Title" --lazer-data "C:\Users\<username>\AppData\Roaming\osu"
```

### 新規 beatmap set の自動監視

`watch.ps1` は stable の `Songs` フォルダ直下で新規ディレクトリが作成されたことを検知し、5 秒待機してからそのフォルダを importer へ渡します。既存の Songs 全体を走査しません。

1. `dotnet build -c Release -warnaserror` を実行します。
2. `watch.ps1` の `$SongsPath` を自分の stable `Songs` フォルダへ変更します。
3. lazer を完全に終了した状態で、リポジトリ直下から実行します。

```powershell
.\watch.ps1
```

停止するには `Ctrl+C` を押します。lazer が起動中、書き込みが 5 秒以内に完了しない場合、または import が失敗した場合は、watch script は自動再試行しません。終了コードを確認して、lazer を閉じた後に対象フォルダを手動で再実行してください。

stable の Songs フォルダと lazer のデータフォルダは、同じ書き込み可能な NTFS ボリューム上に置く必要があります。release または Debug の lazer IPC endpoint が起動している間は、ツールは実行を拒否します。

## ビルド

```powershell
dotnet build -c Release -warnaserror
```

依存 package のバージョンは意図的に固定されています。`ppy.osu.Game*` package はすべて同時に更新し、実ユーザーデータで使う前に対応する lazer release との互換性を検証してください。

## 配布

このリポジトリで配布するのは source code のみです。コンパイル済み executable、`dotnet publish` の出力、復元済み NuGet runtime package は配布しません。各自で local build を行い、NuGet が restore 時に依存関係を取得します。

コンパイル済み出力を再配布したり、商用利用したりする前に、すべての依存関係の licence 条件を個別に確認してください。特に lazer game resources には非商用および font の licence 条件があります。詳細は下記の第三者通知を参照してください。

## ライセンスと商標

このプロジェクトが作成した source code は [MIT License](LICENSE) で提供します。この licence は third-party package や asset の利用権を与えるものではありません。osu!lazer の帰属表示と依存関係の義務については [第三者通知](THIRD_PARTY_NOTICES.md) を参照してください。`osu!`、`osu`、`lazer`、ppy の branding は ppy Pty Ltd に帰属し、このリポジトリは公式プロジェクトではありません。