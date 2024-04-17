- [Simscop.Pl](#simscoppl)
  - [硬件设备](#硬件设备)
    - [1. Camera](#1-camera)
    - [2. Motor](#2-motor)
    - [3. Spectrometer](#3-spectrometer)
  - [软件依赖](#软件依赖)
  - [开发清单](#开发清单)
  - [其他](#其他)

# Simscop.Pl

拉曼软件。

## 硬件设备

### 1. Camera

相机采用的公司为 _[ToupTek](https://www.touptekphotonics.com.cn/download/)_.

*NOTE：使用时直接下载USB开发模式即可*

### 2. Motor

电机控制使用的公司为 _[Zaber](https://www.zaber.com/software)_.

*NOTE：其中有一个虚拟设备，可以用来辅助开发*

型号如下，

| Axis |    Model    |
| :--: | :----------: |
|  XY  | X-LSM200A-E0 |
|  Z  |   X-VSR40   |

### 3. Spectrometer

~~光谱仪使用的公司为 _[OceanInsight](https://www.oceaninsight.com/products/software/drivers/oceandirect/)_~~

安装地址为 [omnidriver-and-spam](https://www.oceaninsight.com/support/software-downloads/omnidriver-and-spam/)

Wrapper API文档 [Wrapper](https://www.oceaninsight.com/globalassets/catalog-blocks-and-images/software-downloads-installers/javadocs-api/omnidriver/index.html)

1. 首先安装 `omnidriverspam-2.56-win32-installer.exe`
2. 前往安装目录的 `OOI_HOME` 找到 `NETOmniDriver.dll` 和 `NETSpam.dll` 添加引用

*NOTE：dll找对应环境用的时PATH，所以x86和x64最好只选择一个来安装，不然容易出问题*

## 软件依赖

## 开发清单

|                         名称                         |    版本    |                                                                        下载地址                                                                        |
| :--------------------------------------------------: | :--------: | :-----------------------------------------------------------------------------------------------------------------------------------------------------: |
|                    toupcamsdk-usb                    | 2024-01-21 |                                           https://www.touptekphotonics.com.cn/static/software/toupcamsdk.zip                                           |
|                 Zaber Motion Library                 |     -     |                 [Getting Started - Zaber Motion Library - Zaber Software Portal](https://software.zaber.com/motion-library/docs/tutorials)                 |
|                 Zaber Virtual Device                 |     -     |                               [Zaber Virtual Device - Zaber Software Portal](https://software.zaber.com/virtual-device/home)                               |
|               OceanDirect Sample Pack               |     -     |      https://www.oceaninsight.com/globalassets/catalog-blocks-and-images/catalog-blocks-and-images/software/oceandirect/oceandirectsamplepack.zip      |
| Tech Note - High Speed Averaging Mode with Ocean SR2 |     -     |         https://www.oceaninsight.com/globalassets/catalog-blocks-and-images/app-notes/tech-note---high-speed-averaging-mode-with-ocean-sr2.pdf         |
|       MNL-1025 OceanDirect User Manual 060822       |     -     | https://www.oceaninsight.com/globalassets/catalog-blocks-and-images/manual--instruction-re-branded/software/mnl-1025-oceandirect-user-manual-060822.pdf |

## 其他
