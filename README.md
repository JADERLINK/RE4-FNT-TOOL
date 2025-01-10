# RE4-FNT-TOOL
Extract and repack RE4 FNT files [Edit the Sprite Sheet] (RE4 2007/PS2/UHD/PS4/NS/GC/WII/X360)

**Translate from Portuguese Brazil**

Programa destinado a extrair e reempacotar arquivos FNT;

## Sobre o arquivo FNT:

O arquivo FNT é responsável por definir os símbolos que são apresentados na tela para vários idiomas.
<br>O arquivo FNT trabalha em conjunto com o MDT para criar diálogos de texto para o jogador interagir durante o jogo, nos menus e cenas.
<br> Esse arquivo define o espaçamento dos símbolos (glifos), no qual determina as margens das letras da fonte.

## Extract

Exemplo de uso:
<br> => RE4_FNT_TOOL_*.exe "common_p.fnt"

Vai gerar os arquivos:
<br> ! common_p.idxfnt = usado no repack, que contém os valores de espaçamento.
<br> ! common_p.tpl = o arquivo TPL é diferente para cada versão do jogo, pode ser uma referência para a textura ou a própria textura. Isso vai depender da versão do jogo.

## Repack

Exemplo de uso:
<br> => RE4_FNT_TOOL_*.exe "common_p.idxfnt"

Para fazer o repack também é necessário o arquivo TPL de mesmo nome.
<br>Essa ação vai gerar o arquivo de nome "common_p.fnt";

## MakeIdxFntFromPng.exe

Essa tool cria um arquivo idxfnt com os espaçamentos corretos para a "Sprite Sheet" que contém seus símbolos.
<br>Para essa tool funcionar, você precisa de dois arquivos: o arquivo configfnt (que já tem disponível junto com a tool), e uma imagem PNG. Essa é a textura com os caracteres (que no jogo está no formato DDS/TGA/GNF, mas você deve salvá-la como PNG para a tool funcionar.)

Exemplo de uso:
<br> => MakeIdxFntFromPng.exe "0f000001_0000_UHD.configfnt" "0f000001_0000.png"

Vai gerar os arquivos:
<br> ! 0f000001_0000.idxfnt = usado para o repack da outra tool. 
<br> ! 0f000001_0000.check.png = apenas visual. Usado para você ver como a tool definiu os espaçamentos. No jogo, só será exibida a parte do caractere que está na parte laranja da imagem.)

## MakeSpriteSheetInfos.exe

Essa Tool é apenas informativa, recebe como parâmetros os mesmos arquivos que a tool que foi apresentada acima.
<br> Ela vai gerar os arquivos:
<br> ! 0f000001_0000.IDs_Part1.png = essa imagem mostra qual é o ID de cada caractere.
<br> ! 0f000001_0000.IDs_Part2.png = essa é a continuação dos IDs (sim, eles se sobrepõem). Você não deve usar esses IDs, a não ser que saiba o que está fazendo.
<br> ! 0f000001_0000.IDs_Double.png = esse é um exemplo de como ficaria a sequência de IDs com os dois conjuntos de IDs juntos.

## Sobre o arquivo IDXFNT

Nesse arquivo é definido o espaçamento da fonte que determina as margens das letras da fonte.

```
# linhas que começam com # são comentários.
# header são os valores que ficam junto ao cabeçalho (header) do arquivo.
# a função desses valores é desconhecida.
# você pode deixar o valor que está ou pode deixar zerado.
Header2:0012FAE0
# ... Linhas omitidas.
Header7:FFFFFFFF

# FontSpacing define o espaçamento, vai de 0x80 a até quanto você precisar.
FontSpacing_0080_StartPoint:0
FontSpacing_0080_!End!Point:64
# o valor 0080 é o ID do Char
# StartPoint -> pixel de início do caractere (pixel incluso)
# !End!Point -> pixel de término do caractere (pixel não incluso)
# lembrando que a contagem começa em 0
# os valores podem ir de 0 até 127
# aviso: o valor de !End!Point deve sempre ser maior que o de StartPoint;
```

## Sobre o arquivo CONFIGFNT

Nesse arquivo, estão as configurações necessárias para a criação de um arquivo idxfnt correto.
<br> Na pasta junto com o programa, têm arquivos configfnt de referência que podem ser editados. 

```
# linhas que começam com # são comentários

# quantidade de caracteres na Horizontal
HorizontalCharCount:32

# quantidade de caracteres na Vertical
VerticalCharCount:12

# quantos pixels que cada caractere possui na horizontal (internamente no jogo).
# atenção: esse campo tem valores fixos para cada versão do jogo.
# para UHD/PS4/NS o valor é 64
# para 2007/PS2/PS3/Xbox360/GC/WII é 32
# para os idiomas Japanese e Chinese é 28
BaseHorizontalCharLength:64

# Extra_Margin serve para não deixar os caracteres colados um no outro no texto do jogo.
# quantidade de margem (esquerda) extra que o programa vai deixar no caractere:
ExtraLeftMargin:1
# quantidade de margem (direta) extra que o programa vai deixar no caractere:
ExtraRightMargin:1

# tamanho mínimo que um caractere pode ter (em pixel):
MinimumCharLength:12

# o programa verifica onde fica cada caractere pela alfa da textura.
# o valor desse campo é o tanto de alfa que deve ser ignorado:
CheckAlpha:10

# depois, na parte abaixo desse arquivo, pode ter os mesmos campos que o aquivo idxfnt tem.
# Eles servem para sobrepor os valores, caso você queira definir manualmente um espaçamento.
```
Nota: o espaçamento é medido em pixels, mas não necessariamente está na mesma escala de pixel que a imagem de referência. 
<br>Por exemplo, no idioma Chinese o arquivo FNT considera que cada char tem no máximo 28 pixels. Mas no arquivo de textura, na verdade, tem 58 pixels (o dobro). Então, ao contar as posições na imagem nesse caso, você deve dividir o valor obtido por 2.
<br>Atenção: O programa faz esse cálculo automaticamente. Então, o arquivo idxfnt gerado já está pronto para uso (no caso, fazer o repack para o arquivo FNT).

**At.te: JADERLINK**
<br>2025-01-10