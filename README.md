# SeaweedFSProject


## master 服务启动，用localhost有问题，建议使用ip
```bash
weed master
weed master -ip=192.168.0.59
```
## master节点设置好了之后，再设置volume节点,volume节点启动
> weed volume -dir="E:/worksoftware/weed/data" -max=10 -mserver="192.168.0.59:9333" -port=9000  -dataCenter=dc1 -index=leveldb
> weed volume -dir="./data" -max=10 -mserver="192.168.1.37:9333" -port=9000 -index=leveldb
> weed volume -dir="E:/worksoftware/weed/tmp/data1" -max=5  -mserver="localhost:9333" -dataCenter=dc1 -port=9098 &
> weed volume -dir="E:/worksoftware/weed/tmp/data2" -max=10 -mserver="localhost:9333" -dataCenter=dc2 -port=9099 &

## 查看是否启动成功：
http://localhost:9333
