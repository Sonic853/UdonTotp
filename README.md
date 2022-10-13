# UdonTotp

![cb794b20-4186-47d8-9206-558db3fca6e81](https://user-images.githubusercontent.com/8389962/194811277-8a9ad310-75d7-4cbc-9413-52509b08fa20.png)

基于时间的一次性密码（TOTP）兼容VRChat UdonSharp

Demo：[https://vrchat.com/home/world/wrld_e47376a2-3fba-484d-b08f-3719aa18b18c](https://vrchat.com/home/world/wrld_e47376a2-3fba-484d-b08f-3719aa18b18c)

## 需要的资源

* Unity 2019.4.31f1
* [VRChat SDK - Base 3.1.7](https://github.com/Tree-Roots/VRChatPackages)
* [VRChat SDK - Worlds 3.1.7](https://github.com/Tree-Roots/VRChatPackages)
* [UdonSharp 1.1.1](https://github.com/Tree-Roots/VRChatUdonSharpPackages) （v0.20.3 以及以下不受技术支持）

## 如何使用？

1. 安装 [release](https://github.com/Sonic853/UdonTotp/releases/latest) 里最新的 unitypackage
2. 安装插件后，使用 [TotpGen插件](https://853lab.booth.pm/items/4141499) 或免费的 [totp-wasm](https://totp-wasm.vercel.app/) （来自[Gizmo](https://github.com/GizmoOAO)） 生成密钥
3. 将示例“TotpKeypad”放入世界
4. 为“TotpKeypad”里的“TOTP”脚本配置编译后密钥（secret）、时间间隔、验证码位数、容错倍数
5. 全部完成！

## 许可证

代码在 [MIT 许可](./LICENSE) 下分发，只要您注明了该插件的著名，您就可以在您的专有项目中随意使用它。

# English

Time-based One-time Password algorithm (TOTP) Compatible with VRChat UdonSharp.

Demo: [https://vrchat.com/home/world/wrld_e47376a2-3fba-484d-b08f-3719aa18b18c](https://vrchat.com/home/world/wrld_e47376a2-3fba-484d-b08f-3719aa18b18c)

## Requirements

* Unity 2019.4.31f1
* [VRChat SDK - Base 3.1.7](https://github.com/Tree-Roots/VRChatPackages)
* [VRChat SDK - Worlds 3.1.7](https://github.com/Tree-Roots/VRChatPackages)
* [UdonSharp 1.1.1](https://github.com/Tree-Roots/VRChatUdonSharpPackages) (v0.20.3 and below are not supported)

## How to use ?

1. Install the latest unitypackage in [release](https://github.com/Sonic853/UdonTotp/releases/latest).
2. After installing the plugin, generate a secret key using [TotpGen plugin](https://853lab.booth.pm/items/4141499) or free [totp-wasm](https://totp-wasm.vercel.app/) (from [Gizmo](https://github.com/GizmoOAO)).
3. Place the example "TotpKeypad" in the Scene.
4. Configure the secret, period, digits, and tolerance for the "TOTP" script in "TotpKeypad".
5. All done!

## License

The code is distributed under the [MIT License](./LICENSE), as long as you indicate the attribution of the plugin, you can use it freely in your proprietary projects.
