# Agile Server 2.0
适配于Agile功能套件

## 开发人员使用

1. 第一次使用时，请更新代码到最新的版本，然后再项目主目录下运行bootstrap.bat
2. 打开项目目录中的sln文件，进入Visual Studio，点击DEBUG运行
3. 点击select path按钮选择项目启动文件夹下的example目录
4. 选择ISP和Model，**注意顺序，先选择ISP，在选择Model**
5. 点击start启动服务器，此时服务器已经启动
6. tftp的访问地址是“IP+route”形式，例如，你需要下载fake.txt文件，server的ip地址是192.168.0.1，那么下载的命令为


```shell
tftp -g -l fake.txt -r /Agile/fake.txt 192.168.0.1
```

## 服务器配置

### DHCP配置

1. start ip: 默认192.168.2.20，支持合理的ip起始地址
2. end ip：默认192.168.2.254，支持合理的ip起始地址
3. AddressTime：续约超时时间，默认300s

支持标准的Option配置，有其余需要配置的，可以额外添加

### TFTP配置

## conf.bin生成配置
