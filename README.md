# Online Dungeon FPS
An online dungeon fps developed with u3d.
# File Structure
- |--Tool contains protobuff binary file.
- |--Src
  - |--Client contains the u3d local client src code.
  - |--Server contains the server src code.
  - |--Data contains the excel tables and json files that are generated from the tables.
  - |--Lib contains the common lib file used by client and server.
# Development Environment
- Unity Editor version 2020.3.26f1c1
- SQL Server version 17.9
- Entity Framework 6 version 6.20
- protobuf version 3.2.0
# Third-party Free Resources
- [FPS Microgame and its Add-on]()
- [Simple Menu](https://assetstore.unity.com/packages/tools/gui/simple-menu-154642)
- [RPG Poly Pack - Lite](https://assetstore.unity.com/packages/3d/environments/landscapes/rpg-poly-pack-lite-148410#content)
# Database Generation
- You need to modify the connection string in \Src\Server\GameServer\GameServer\App.config.
- You should generate the database by EF6 model.