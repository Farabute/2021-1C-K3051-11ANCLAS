﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Ships;
using Microsoft.Xna.Framework.Audio;


namespace TGC.MonoGame.TP.Bullet
{
    public class Bullet
    {

        private float _bulletSpeed = 25f;
        private float _timeElapsed;
        private Vector3 _destinationPosition;
        private float _lifeTime = 2000;
        private Vector3 _direccion;
        private TGCGame _game;
        private Camera _camera;
        private Ship _barcoOrigen;
        private bool _searchImpact;
        public bool _active;
        public bool _available;

        public Bullet()
        {
            _available = true;
        }


        public void Init(TGCGame tgcGame, Vector3 origin, Vector3 direction, Ship barco)
        {
            _game = tgcGame;
            _camera = _game.CurrentCamera;
            _destinationPosition = origin;
            _direccion = direction;
            _timeElapsed = 0;
            _searchImpact = false;
            _active = true;
            _barcoOrigen = barco;

        }

        public void Update()
        {

            if (_timeElapsed > _lifeTime || Impact(_destinationPosition) || _destinationPosition.Y < -50f)
            {

                _searchImpact = false;
                _active = false;
                _available = true;

            }
        }

        public void Draw(GameTime gameTime)
        {
            _game.BulletModel.Draw(_game.World * Matrix.CreateTranslation(_destinationPosition), _camera.View, _camera.Projection);
            _destinationPosition += _direccion * _bulletSpeed;
            _destinationPosition.Y -= _timeElapsed / 90;
            _timeElapsed += 1;
            _searchImpact = true;

        }

        private bool Impact(Vector3 destination)
        {
            bool willCollide = false;

            for (var index = 0; index < _game.IslandColliders.Length && !willCollide; index++)
            {
                BoundingSphere FuturePosition = new BoundingSphere(destination, 1f);
                if (FuturePosition.Intersects(_game.IslandColliders[index]))
                {
                    willCollide = true;
                }
            }
                if (_searchImpact && !willCollide)
            {
                BoundingSphere FuturePosition = new BoundingSphere(destination, 1f);

                for (var index = 0; index < _game.Ships.Length && !willCollide; index++)
                {

                    if (FuturePosition.Intersects(_game.Ships[index].BoatSphere) && _game.Ships[index] != _barcoOrigen)
                    {
                        if(_game.Ships[index] == _game.PlayerBoat)
                        {
                            if(!_game.godModeEnabled)
                            {
                              _game.PlayerControlledShip._currentLife -= 1;
                            }

                        }
                        if (_barcoOrigen == _game.PlayerBoat)
                        {
                            _barcoOrigen._score += 100;
                            /*if(_barcoOrigen._score % 1000 == 0)
                            {
                                for (var i = 0; i < _game.Ships.Length; i++)
                                {
                                    _game.Ships[index].MovementSpeed += 1000f;
                                    _game.Ships[index].Speed += 1000f;
                                    _game.Ships[index].BoatAcceleration += 10f;
                                }
                            }*/
                        }
                        _game.ExplosionInstance.Stop();
                        _game.ExplosionInstance.Play();
                        _game.ExplosionInstance.Volume = 0.12f;
                        _game.Ships[index].RecibirDanio(50);
                        willCollide = true;
                    }
                }
            }

            return willCollide;
        }
    }
}
