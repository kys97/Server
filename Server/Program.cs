using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

//실제 서버 역할
namespace ServerStudy
{
    class FastLock
    {
        public int id;
    }
    class SessionManager
    {
        FastLock l;
        static object _lock = new object();

        public static void TestSession()
        {
            lock (_lock)
            {

            }
        }

        public static void Test()
        {
            lock (_lock)
            {
                UserManager.TestUser();
            }
        }
    }

    class UserManager
    {
        FastLock l;
        static object _lock = new object();

        public static void Test()
        {
            
            lock (_lock)
            {
                SessionManager.TestSession();
            }
        }

        public static void TestUser()
        {
            lock (_lock)
            {

            }
        }
    }

    class Program
    {
        static int number = 0;
        static object obj = new object();

        static void Thread_1()
        {
            for (int i = 0; i < 10000; i++)
            {
                SessionManager.Test();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 10000; i++)
            {
                UserManager.Test();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();

            Thread.Sleep(100);

            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }


        void Thread_note()
        {
            /* Thread */

            //thread.Start();
            //thread 실행

            //thread.Join();
            //thread가 끝날때까지 기다린 후 다음 코드 실행

            //thread.Name = "Test Thread";
            //thread 이름 지정

            //thread.IsBackground = true;
            //C#의 thread는 기본적으로 ForeGround로 생성.
            //따라서thread가 종료되기 전까지 원래 프로그램은 돌아가지 않음.
            //그러나 IsBackGround를 true로 지정시 프로그램은 이어서 진행되며
            //thread는 프로그램이 종료될때까지 뒤에서 계속 돌아감


            /* ThreadPool */

            //ThreadPool.QueueUserWorkItem(thread_function);
            //Thread를 무수히 많이 생성해야 하는 경우,
            //일일히 Thread를 생성하는것보다 ThreadPool을 이용하는 것이 좋음

            //ThreadPool.SetMinThreads(1, 1); - ThreadPool 최소 Thread 개수 1개로 지정.
            //ThreadPool.SetMaxThreads(5, 5); - ThreadPool 최대 Thread 개수 5개로 지정.


            /* Task */

            //Thread와 ThreadPool의 장점만 이용
            //Task t = new Task(thread_function, TaskCreationOptions.LongRunning);
            //TaskCreationOptions.LongRunning : 해당 task가 작업이 오래 걸림을 의미
            //이러러 경우 기존 Thread와 분리해서 따로 작업함.

            //task.Wait();
            //Thread의 Join과 동일한 기능.
            //task가 끝날때까지 기다렸다가 다음 코드 실행
        }
        void Compiler_note()
        {
            //개발은 Debug모드에서 Test해보지만
            //실제 환경에서는 Release 모드로 실행됨.
            //Release 모드는 코드를 자동으로 최적화 하여 실행하는 모드인데
            //이럴 경우 최적화로 인해 일어나지 않던 버그들이 발생할 수 있음
            //따라서 변수 앞에 volatile 을 붙여 해당 변수는 최적화되지 않도록 할 수 있음

            //최적화 코드는 break Point 설정 하고 프로젝트 실행 후
            //디버그 > 창 > 디스어셈블리 를 통해 어셈블리어 확인 가능
        }
        void MemoryBarrier_note()
        {
            //컴파일러와 마찬가지로 하드웨어 또한 코드를 최적화함.
            //예를들어 스레드 안에서 전혀 상관없는 코드가 있을 경우
            //해당 코드의 줄을 임의로 재배치 하여 실행함
            //따라서 이를 막기 위해 메모리 베리어 사용

            //Thread.MemoryBarrier();
            //깃허브에 pull과 push 느낌.
            //메모리 베리어를 만나는 순간
            //메모리에 있는 데이터를 읽어온 다음에 다음 코드 진행 - pull
            //또는
            //지금까지 코드를 메모리에 저장 - push

            //메모리 베리어 - Store와 Load 둘 다 막음
            // a) 코드 재배치 억제
            // b) 가시성
        }
        void Interlocked_note()
        {
            /* Interlocked */
            //원자성(?)
            //작업을 실행하는 최소한의 단위성

            //++ 연산자의 경우 최적화를 통하면
            // 값 복사, 증가, 저장 의 3단계로 나눠지게 된다
            //따라서 이를 막기 위해 Interlocked 계열의 함수들을 사용

            //Interlocked.Increment(ref number);
            //number 변수의 값을 참조(ref)하여 1증가하는 작업을 atomic화 하여 실행한다.
            //변경된 number의 값을 반환

            //Interlocked.Decrement(ref number);
            //number 변수의 값을 참조(ref)하여 1감소하는 작업을 atomic화 하여 실행한다.
            //변경된 number의 값을 반환
        }
        void Lock_기초()
        {
            //1. ++연산자 뿐 아니라 여러 줄의 코드가 스레드에서 실행되어야 할때
            //2. ++연산자를 사용하는 변수를 Interlocked를 사용하지 않은 다른 줄에서 사용될때

            //=> 이를 해결하기 위해 시작과 끝의 줄을 그어줌
            //Monitor.Enter(obj); //문 잠구는 행위
            //Monitor.Exit(obj); //문 잠금 푸는 행위
            //obj : 문 열쇠같은 느낌

            //상호배제 : Mutual Exclusive

            //문제
            //1. 관리가 어려움
            //잠금 사이에 return이 있을 경우
            //문을 풀어주지 않았기 때문에 프로그램이 멈춤 > DeadLock 발생
            //Enter와 Exit를 수동으로 짝을 맞춰줘야 하기 때문
            //2. Exception 발생 시
            //에러 발생하면 또 DeadLock 발생
            //=> 이를 해결하기 위해 tyr-finally를 사용
            //그러나 이 방법도 여간 번거로움

            //따라서 이를 사용하기 위해
            /*
            lock (obj)
            {
                //코드
            }
            */
            //위와 같이 사용

        }
        void DeadLock()
        {
            //DeadLock
            //서로 다른 lock에서 자물쇠를 호출했을 시 DeadLock 발생
            //추후에 클래스가 많아지면 어디서 DeadLock이 발생했는지 일일히 찾기가 힘들고
            //DeadLock이 발생하지 않도록 관리하기도 어려움

            //꼼수1 : Monitor.TryEnter(obj);
            //일정시간동안 obj가 잠금해제 되지 않았을 시 true 혹은 false 값 리턴
            //그러나 이 방법은 이미 DeadLock이 발생한 후에 넘기는 방식이라 근본적 해결 방법이 아님
            //따라서 방법 비추천

            //이러한 DeadLock 상황은 개발단계가 아닌 라이브때 자주 발생

            //꼼수2 : lock을 Class로 맵핑
            /*
            class FastLock{
                int id;
            }
            */
            //각 클래스에서 lock을 사용하는 id 값을 Enum등으로 Define을 해놓은 경우
            //id값에 따라 lock 사용
            //실용적?인지는 모르겠으며 DeadLock이 아예 사라지진 않음

            //위에 꼼수들은 DeadLock을 완전히 막을 수 있는 방법이 아닌
            //개발단계에서 DeadLock이 어디서 발생하는지 조금이라도 찾아낼 수 있는 방법들

            //DeadLock은 완전히 막을 수 없다.
            //따라서 라이브로 출시한 다음 문제점을 찾아 고치는 방법이 가장 최선
        }
    }

}