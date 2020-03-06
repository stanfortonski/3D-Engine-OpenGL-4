/* Copyright (c) 2020 by Stan Fortoński */

#ifndef ENGINE_HPP
#define ENGINE_HPP 1
#include <GL/glew.h>
#include <stdexcept>
#include <GLFW/glfw3.h>
#include "support/Singleton.hpp"
#include "Config.hpp"

namespace Engine
{
  class Engine: public Singleton<Engine>
  {
    friend class Singleton<Engine>;
    float lastTimeFromShow = 0;
    unsigned frames = 0;
    float lastTime = 0;
    float deltaTime = 0;
    std::string actualFPS;

    Engine(){;}

  public:
    void initGLFW() const;
    void initGLEW() const;
    void initDefaultOptionsGL() const;
    void initDeltaTime();
    void calcFPS();

    #if DEBUG_ENGINE == 1
    void showErrors();
    #endif

    float getDeltaTime() const{return deltaTime;}
    std::string getFPS() const{return actualFPS;}
  };
}
#endif
