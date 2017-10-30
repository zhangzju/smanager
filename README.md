# Agile Server 2.0
适配于Agile功能套件

## 开发人员使用

1. 第一次使用时，请更新代码到最新的版本，然后再项目主目录下运行bootstrap.bat
2. 打开项目目录中的sln文件，进入Visual Studio，点击DEBUG运行
3. 点击select path按钮选择项目启动文件夹下的example目录
4. 选择ISP和Model，**注意顺序，先选择ISP，在选择Model**
5. 点击start启动服务器，此时服务器已经启动
6. tftp的访问地址是“IP+route”形式，例如，你需要下载fake.txt文件，server的ip地址是192.168.0.1，那么瞎子啊的命令式
```shell
tftp -g -l fake.txt -r /Agile/fake.txt 192.168.0.1
```

## 服务器配置

## conf.bin生成配置
