# Projeto Final Capstone Programação 3

 **Integrantes**: **Davi Pereira Lopes** e **Ênio Martins**


![tela_do_jogo](/Assets/Images/logo.png)

---------------------


## Sobre o Projeto



# Pac-Man Clone - Uno Platform

Este projeto é uma recriação funcional do clássico jogo arcade **Pac-Man**, desenvolvido como trabalho prático para a disciplina de Programação da **Jala University**. 

O foco do projeto foi a implementação de lógicas de colisão (matriz estática e cálculo de distância), movimentação contínua, inteligência básica de perseguição dos fantasmas e manipulação de interface gráfica.

## Tecnologias Utilizadas
* **Linguagem:** C#
* **Framework Interface:** Uno Platform
* **Plataforma Alvo:** .NET 10 (Desktop / Windows)

### Nota sobre o Áudio
Para manter o projeto extremamente leve e não depender de pacotes ou bibliotecas externas pesadas, todo o sistema de som (waka-waka, música de fundo, efeitos de colisão) foi construído utilizando o `System.Media.SoundPlayer` **nativo do próprio Windows**.

---

## Como Executar o Jogo

1. Certifique-se de ter o [SDK do .NET 10](https://dotnet.microsoft.com/download) instalado na sua máquina (ou a versão correspondente que deseja utilizar).
2. Clone este repositório ou baixe a pasta do projeto.
3. Abra o seu terminal (CMD, PowerShell ou terminal do VS Code) e navegue até a pasta raiz do projeto (onde está o arquivo `.csproj`).
4. Execute o seguinte comando:

```bash
dotnet run --framework net10.0-desktop


Importante: Versão do .NET
Se você for rodar este projeto utilizando outra versão do .NET (por exemplo, .NET 8 ou .NET 9), o comando acima falhará.
Nesse caso, você precisa:
Abrir o arquivo pacman.csproj (ou o nome que estiver no seu arquivo de projeto).
Encontrar a tag <TargetFrameworks> (ou <TargetFramework>).
Alterar de net10.0-desktop para a versão instalada na sua máquina (ex: net8.0-desktop).
Rodar o comando correspondente (ex: dotnet run --framework net8.0-desktop).
```
### Desenvolvedores
```bash
Projeto desenvolvido com muita dedicação por:
Davi Pereira Lopes
Ênio Martins
Jala University
```