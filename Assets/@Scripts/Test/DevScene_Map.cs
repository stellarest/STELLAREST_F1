using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using static STELLAREST_F1.Define;

#if UNITY_EDITOR
namespace STELLAREST_F1
{
    public class DevScene_Map : BaseScene
    {
        public int TestEnvDataID = 0;
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            SceneType = Define.EScene.Unknown;
            LoadAsset();
            return true;
        }

        private IEnumerator CoContinuousSpawnMonster_Test(float waitTime = 0.1f)
        {
            while (true)
            {
                yield return new WaitUntil(() => Managers.Object.Monsters.Count == 0);
                yield return new WaitForSeconds(waitTime);
                Monster chicken = Managers.Object.SpawnBaseObject<Monster>
                    (objectType: EObjectType.Monster, spawnPos: Managers.Map.CellToCenteredWorld(new Vector3Int(-9, 8, 0)),
                     dataID: ReadOnly.DataAndPoolingID.DNPID_Monster_Chicken);
            }
        }

        private bool _spawnEnvTypeFlag = false;
        private IEnumerator CoContinuousSpawnEnv_Test(float waitTime = 0.1f)
        {
            while (true)
            {
                yield return new WaitUntil(() => Managers.Object.Envs.Count == 0);
                yield return new WaitForSeconds(waitTime);
                if (_spawnEnvTypeFlag == false)
                {
                    Env env = Managers.Object.SpawnBaseObject<Env>(EObjectType.Env,
                       spawnPos: Managers.Map.CellToCenteredWorld(new Vector3Int(-6, 11, 0)),
                       dataID: ReadOnly.DataAndPoolingID.DNPID_Env_AshTree);
                }
                else
                {
                    Env env = Managers.Object.SpawnBaseObject<Env>(EObjectType.Env,
                       spawnPos: Managers.Map.CellToCenteredWorld(new Vector3Int(-3, 11, 0)),
                       dataID: ReadOnly.DataAndPoolingID.DNPID_Env_GoldRock);
                }

                _spawnEnvTypeFlag = !_spawnEnvTypeFlag;
            }
        }

        private void SpawnChicken_Test(int cellPosX, int cellPosY)
        {
            Managers.Object.SpawnBaseObject<Monster>
                (objectType: EObjectType.Monster, spawnPos: Managers.Map.CellToCenteredWorld(new Vector3Int(cellPosX, cellPosY, 0)),
                dataID: ReadOnly.DataAndPoolingID.DNPID_Monster_Chicken, owner: null);
        }

        private void Test()
        {
            UI_Joystick joystick = Managers.UI.ShowBaseUI<UI_Joystick>();
            Managers.Map.LoadMap("SummerForestField_Test2");
            Managers.Map.Map.transform.position = Vector3.zero;

            {
                // --- Lead er Controlelr, Cam
                HeroLeaderController leaderController = Managers.Object.SpawnHeroLeaderController();
                CameraController cam = Camera.main.GetComponent<CameraController>();
                Managers.Object.CameraController = cam;

                // --- First Hero
                Hero firstHero = Managers.Object.SpawnBaseObject<Hero>(objectType: EObjectType.Hero,
                    spawnPos: Vector3.zero,
                    dataID: ReadOnly.DataAndPoolingID.DNPID_Hero_Archer);
                leaderController.Leader = firstHero;

                StartCoroutine(CoContinuousSpawnMonster_Test(2.5f));
                // StartCoroutine(CoContinuousSpawnEnv_Test(1f));

                {
                    // SpawnChicken_Test(-9, 9);
                    // SpawnChicken_Test(-10, 8);
                    // SpawnChicken_Test(-8, 8);
                    // SpawnChicken_Test(-9, 7);

                    // SpawnChicken_Test(-10, 9);
                    // SpawnChicken_Test(-10, 7);
                    // SpawnChicken_Test(-8, 9);
                    // SpawnChicken_Test(-8, 7);

                    // SpawnChicken_Test(-6, 7);
                    // SpawnChicken_Test(-6, 6);
                    // SpawnChicken_Test(-6, 5);
                    // SpawnChicken_Test(-6, 9);

                    // SpawnChicken_Test(-4, 7);
                    // SpawnChicken_Test(-4, 6);
                    // SpawnChicken_Test(-4, 5);

                    // SpawnChicken_Test(-3, 8);
                    // SpawnChicken_Test(-3, 7);
                    // SpawnChicken_Test(-3, 6);
                    // SpawnChicken_Test(-3, 5);
                    // SpawnChicken_Test(-3, 4);

                    // SpawnChicken_Test(-7, 8);
                    // SpawnChicken_Test(-7, 7);
                    // SpawnChicken_Test(-7, 6);
                    // SpawnChicken_Test(-7, 5);
                    // SpawnChicken_Test(-7, 4);
                    SpawnChicken_Test(-7, 10);
                    SpawnChicken_Test(-8, 9);

                    // SpawnChicken_Test(-6, 10);
                    // SpawnChicken_Test(-6, 11);
                    // SpawnChicken_Test(-5, 9);
                    // SpawnChicken_Test(-5, 6);
                    // SpawnChicken_Test(-7, 9);
                    // SpawnChicken_Test(-7, 6);
                    // SpawnChicken_Test(-4, 9);
                    // SpawnChicken_Test(-4, 6);
                    // SpawnChicken_Test(-5, 5);
                    // SpawnChicken_Test(-7, 7);
                    // SpawnChicken_Test(-7, 5);

                    // SpawnChicken_Test(-3, 3);
                    // SpawnChicken_Test(-5, 4);
                    // SpawnChicken_Test(-1, 4);
                    // SpawnChicken_Test(-5, 2);
                    // SpawnChicken_Test(-1, 2);
                }

                // --- Env
                // Env env = Managers.Object.SpawnBaseObject<Env>(EObjectType.Env, 
                //     spawnPos: Managers.Map.GetCenterWorld(new Vector3Int(-6, 11, 0)), 
                //     dataID: ReadOnly.DataAndPoolingID.DNPID_Env_AshTree);

                // --- HEROES TEST
                // RandPos Test
                // int attemptCount = 0;
                // Vector3Int randPos = new Vector3Int(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                // while (Managers.Map.CanMove(randPos) == false && attemptCount++ < 100)
                // {
                //     if (attemptCount >= 100)
                //     {
                //         Debug.LogError("Failed set randPos");
                //         Application.Quit();
                //     }
                //     randPos = new Vector3Int(Random.Range(-3, 3), Random.Range(-3, 3), 0); // Retry
                // }

                // HeroLeaderController leaderController = Managers.Object.SetHeroLeaderController();
                // CameraController cam = Camera.main.GetComponent<CameraController>();
                // Managers.Object.CameraController = cam;
                // Hero firstHero = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Paladin);
                //Managers.Map.MoveTo(firstHero, Vector3.zero, stopLerpToCell: true, forceMove: true);

                // // Leader:1, Maximum Members:6
                // // Paladin - Archer, Wizard, Lancer, Gunner
                // int memberCount = 0;
                // int memberMaxCount = 1;
                // // int dataID = ReadOnly.DataAndPoolingID.DNPID_Hero_Archer;
                // int dataID = ReadOnly.DataAndPoolingID.DNPID_Hero_Paladin;
                // while (memberCount < memberMaxCount)
                // {
                //     randPos = new Vector3Int(Random.Range(-4, 4), Random.Range(-4, 4), 0);
                //     if (Managers.Map.CanMove(randPos) == false)
                //         continue;

                //     memberCount++;
                //     Hero hero = Managers.Object.Spawn<Hero>(EObjectType.Hero, dataID);
                //     //dataID -= 10;

                //     hero.gameObject.name += $"___{memberCount.ToString()}";
                //     Managers.Map.MoveTo(hero, randPos, stopLerpToCell: true, forceMove: true);
                //     hero.InitialSpawnedCellPos = randPos;
                // }
                // leaderController.Leader = firstHero;
                // int paladin = 0;
                // int archer = 0;

                // int lancer = 0;
                // int wizard = 0;

                // int assassin = 0;
                // int gunner = 0;

                // int trickster = 0;
                // int druid = 0;

                // int barbarian = 0;
                // int ninja = 0;

                // int phantomKnight = 0;
                // int frostWeaver = 0;

                // int queen = 0;
                // int hunter = 0;

                // int gladiator = 0;
                // int priest = 0;

                // int berserker = 0;
                // int witch = 0;

                // int dragonKnight = 0;
                // int alchemist = 0;

                // int current = 0;
                // int total = paladin + archer + lancer + wizard + assassin + gunner + trickster + druid + barbarian + ninja + phantomKnight + frostWeaver + 
                //             queen + hunter + gladiator + priest + berserker + witch + dragonKnight + alchemist;

                // Hero heroMember = null;
                // while (current < total)
                // {
                //     randPos = new Vector3Int(Random.Range(-4, 4), Random.Range(-4, 4), 0);
                //     if (Managers.Map.CanMove(randPos) == false)
                //         continue;

                //     if (paladin > 0)
                //     {
                //         paladin--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Paladin);
                //     }
                //     else if (archer > 0)
                //     {
                //         archer--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Archer);
                //     }
                //     else if (lancer > 0)
                //     {
                //         lancer--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Lancer);
                //     }
                //     else if (wizard > 0)
                //     {
                //         wizard--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Wizard);
                //     }
                //     else if (assassin > 0)
                //     {
                //         assassin--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Assassin);
                //     }
                //     else if (gunner > 0)
                //     {
                //         gunner--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Gunner);
                //     }
                //     else if (trickster > 0)
                //     {
                //         trickster--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Trickster);
                //     }
                //     else if (druid > 0)
                //     {
                //         druid--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Druid);
                //     }
                //     else if (barbarian > 0)
                //     {
                //         barbarian--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Barbarian);
                //     }
                //     else if (ninja > 0)
                //     {
                //         ninja--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Ninja);
                //     }
                //     else if (phantomKnight > 0)
                //     {
                //         phantomKnight--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_PhantomKnight);
                //     }
                //     else if (frostWeaver > 0)
                //     {
                //         frostWeaver--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_FrostWeaver);
                //     }
                //     else if (queen > 0)
                //     {
                //         queen--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Queen);
                //     }
                //     else if (hunter > 0)
                //     {
                //         hunter--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Hunter);
                //     }
                //     else if (gladiator > 0)
                //     {
                //         gladiator--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Gladiator);
                //     }
                //     else if (priest > 0)
                //     {
                //         priest--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Priest);
                //     }
                //     else if (berserker > 0)
                //     {
                //         berserker--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Berserker);
                //     }
                //     else if (witch > 0)
                //     {
                //         witch--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Witch);
                //     }
                //     else if (dragonKnight > 0)
                //     {
                //         dragonKnight--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_DragonKnight);
                //     }
                //     else if (alchemist > 0)
                //     {
                //         alchemist--;
                //         heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Alchemist);
                //     }

                //     current++;
                //     heroMember.gameObject.name += $"___{current.ToString()}";
                //     Managers.Map.MoveTo(heroMember, randPos, stopLerpToCell: true, forceMove: true);
                // }

                // leaderController.Leader = firstHero;
            }

            {   // --- ENV SINGLE TEST
                // // DNPID_Env_CopperRock
                // // // // Temp - Spawn Env (스폰 데이터 시트로 빼야함)
                // Env env = Managers.Object.Spawn<Env>(EObjectType.Env, ReadOnly.DataAndPoolingID.DNPID_Env_AshTree);
                // // int x = Managers.Object.Monsters[0].CellPos.x;
                // // int y = Managers.Object.Monsters[0].CellPos.y;
                // //Vector3Int randPos = new Vector3Int(Random.Range(x + 3, x + 5), Random.Range(y + 3, y + 5));
                // Vector3Int randPos = new Vector3Int(-6, 11, 0);
                // env.SetCellPos(cellPos: randPos, stopLerpToCell: true, forceMove: true);
                // env.UpdateCellPos();
            }

            {
                // // --- ENV SPREAD
                // int envCount = 0;
                // int envMaxCount = 0;
                // int minX = Managers.Map.MinX;
                // int maxX = Managers.Map.MaxX;
                // int minY = Managers.Map.MinY;
                // int maxY = Managers.Map.MaxY;
                // int attempCount = 0;
                // Vector3Int cellPos = new Vector3Int(Random.Range(minX, maxX), Random.Range(minY, maxY));
                // while (envCount < envMaxCount)
                // {
                //     if (attempCount++ > 100)
                //         break;

                //     if (Managers.Map.CanMove(cellPos) == false)
                //     {
                //         cellPos = new Vector3Int(Random.Range(minX, maxX), Random.Range(minY, maxY));
                //         continue;
                //     }

                //     bool envTree = true;
                //     if (Random.Range(0, 100) > 50)
                //         envTree = false;

                //     int spawnDataID = -1;
                //     if (envTree)
                //         spawnDataID = GetRandEnvTree;
                //     else
                //         spawnDataID = GetRandEnvRock;

                //     Env env = Managers.Object.Spawn<Env>(EObjectType.Env, spawnDataID);
                //     env.SetCellPos(cellPos: cellPos, stopLerpToCell: true, forceMove: true);
                //     env.UpdateCellPos();
                //     ++envCount;
                // }
            }

            if (Managers.Object.HeroLeaderController == null)
            {
                Debug.LogError("This game absolutely requires at least one Hero Leader Controller.");
                Application.Quit();
            }
        }

        private int GetRandEnvTree
            => UnityEngine.Random.Range(ReadOnly.DataAndPoolingID.DNPID_Env_AshTree, ReadOnly.DataAndPoolingID.DNPID_Env_YewTree + 1);

        private int GetRandEnvRock
            => UnityEngine.Random.Range(ReadOnly.DataAndPoolingID.DNPID_Env_CopperRock, ReadOnly.DataAndPoolingID.DNPID_Env_ZincRock + 1);

        private void LoadAsset()
        {
            Managers.Resource.LoadAllAsync<Object>(label: ReadOnly.Util.PreLoad, callback: delegate (string key, int count, int totalCount)
            {
                Debug.Log($"Key Loaded : {key}, Current : {count} / Total : {totalCount}");
                if (count == totalCount)
                {
                    Managers.Data.Init();
                    //Managers.Sprite.Init();
                    //Managers.Animation.Init();
                    Test();
                }
            });
        }

        public override void Clear() { }
    }
}
#endif