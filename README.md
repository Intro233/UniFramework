# UniFramework

Unity简单框架。(工具集，还谈不上框架)

## 安装
1. 将UniFramework\Luban和Assets目录下所有文件复制到工程
2. 在项目`manifest.json`文件中添加依赖包：` "com.unity.scriptablebuildpipeline": "2.1.3"`

## 依赖关系

### 1.不依赖其余模块

- UniEvent
- ObjectPool
- Timer
- Singleton

### 2. 依赖其他模块

`无序列表二级为所依赖模块`

- UI
  - 快速绑定组件(依赖Odin)

## Requirements

- Odin Inspector and Serializer 3.3+ 商业化用途请自行购买
