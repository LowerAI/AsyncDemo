# P1 线程(Thread)：创建线程
## 什么是线程
+ 线程是一个可执行路径，它可以独立于其他线程执行。
+ 每个线程都会在操作系统的进程(Process)内执行，而操作系统提供看了程序运行的独立环境。
+ 单线程应用，在京城的独立环境里只跑一个线程没所以该线程拥有独占权。
+ 多线程应用，单个进程中会跑多个线程，他们会共享当前的执行环境(尤其是内存)
  + 例如，一个线程在后台读取数据命令一个线程在数据达到后进行展示。

## 例子
+ 在单核计算机上，操作系统必须为每个线程分配“时间片”(在Windows中通常为20毫秒)来模拟并发，从而导致重复的x和y块。
+ 在多核或多任务处理器计算机上，这两个线程可以真正地并行执行(可能受到计算机上其他活动进程的竞争)。
  + 在本例中，由于控制台处理并发请求的机制的微妙性，您仍然会得到重复的x块和y块。

## 术语：线程被抢占
+ 线程在这个时候可以称为被抢占了：
  + 它的执行与零一个线程上代码的执行交织的那一点。

## 线程的一些属性
+ 线程一旦开始执行，IsAlive就是ture，线程结束就变成false。
+ 线程结束的条件就是：线程构造函数传入的委托结束了执行。
+ 线程一旦结束，就无法再重启。
+ 每个线程都有个Name属性，通常用于调试。
 + 线程Name只能设置一次，以后更改会抛出异常。
 + 静态的Thread.CurrentThread属性，会返回当前执行的线程
   + 例子：CurrentThread

# P2 Thread.Join() & Thread.Sleep()
## Join and Sleep
+ 调用Join方法，就可以等待另一个线程结束
+ 

## 添加超时
+ 调用`Join`的时候，可以设置一个超时，用毫秒或者`TimeSpan`都可以。
  + 如果返回`true`，那就是线程结束了；如果超时了，就返回`false`。
+ `Thread.Sleep()`方法会暂停当前的线程，并等待一段时间。

+ 注意：
 + `Thread.Sleep(0)`这样调用会导致线程立即放弃本身当前的时间片，自动将CPU移交给其他线程。
 + `Thread.Yield()`做同样的事情，但是它只会把执行交给同一处理器上的其他线程。
 + 当前`Sleep`或`Join`的时候，线程处于阻塞的状态。

# P3 阻塞
+ 如果线程的执行由于某种原因导致暂定，那么就认为该线程被阻塞了。
  + 例如`Sleep`或者通过`Join`等待其他线程结束。
+ 被阻塞的线程会立即将其处理器的时间片生成给其它线程，从此就不在消耗处理器时间，直到满足其阻塞条件为止。
+ 可以通过`ThreadState`这个属性来判断线程是否处于被阻塞的状态：  
  `bool blocked = (someThread.ThreadState & ThreadState.waitSleepJoin) != 0;`

## ThreadState
+ `ThreadState`是一个`flags enum`，通过按位的形式，可以合并数据的选项。

## 解除阻塞 Unlocking
+ 当遇到下列四种情况的时候，就会解除阻塞：
  + 阻塞条件被满足
  + 操作超时(如果设置超时的话)
  + 通过`Thread.Interrupt()`进行打断
  + 通过`Thread.Abort()`进行中止

## I/O-bound vs Compute-bound(或CPU-bound)
+ 一个花费大部分时间等待某事发生的操作称为I/O-bound
  + I/O绑定操作通常涉及输入输出，但这不是硬性要求：`Thread.Sleep()`也被视为`I/O-bound`
+ 相反，一个花费大部分时间执行CPU密集型工作的操作称为`Compute-bound`

## 阻塞 vs 忙等待(自旋) Blocking vs Spinning
+ I/O-bound 操作的工作方式有两种：
  + 在当前线程上同步的等待
    + Console.ReadLine(),Thread.Sleep(),Thread.Join()...
  + 异步的操作，在稍后操作完成时触发一个回调动作。
+ 同步等待的I/O-bound操作将大部分时间花在阻塞线程上。
+ 他们也可以周期性的在一个循环里进行“打转(自旋)”
  ```
  while (DateTime.Now < nextStartTime)
    Thread.Sleep(100);
  ```
  ```
  while (DateTime.Now < nextStartTime)
  ```
+ 在忙等待和阻塞方面有一些细微差别。
  + 首先，如果您希望条件很快得到满足(可能在几微妙之内)，则短暂自旋可能会很有效，因为它避免了上下文切换的开销和延迟。
    + .NET Framework提供了特殊的方法和类来提供帮助SpinLock和SpinWait。
  + 其次，阻塞也不是零成本。这是因为每个线程在生存期间会占用大约1MB的内存，并会给CLR和操作系统带来持续的管理开销。
    + 因此，在需要处理成百上千个并发操作的大量I/O-bound程序的上下文中，阻塞可能会很麻烦
    + 所以，此类程序需要使用基于回调的方法，在等待时完全撤销其线程。

## 本地 vs 共享的状态 Local vs Shared State
### Local 本地的独立
+ CLR为每个线程分配自己的内存栈(Stack)，以便使本地变量保持独立。

### Shared 共享
+ 如果多个线程都引用到同一个对象的实例，那么他们就共享了数据。
+ 被Lambda表达式或匿名委托所捕获的本地变量，会被编译器转化为字段(field)，所以也会被共享。
+ 静态字段(field)也会在线程间共享数据。

# P4 什么是线程安全
# 线程安全 Thread Safety
+ 后三个例子就引出了线程安全这个关键概念(或者说缺乏线程安全)
+ 上述例子的输出实际上是无法确定的：
  + 有可能(理论上)“Done”会被打印两次。
  + 如果交换Go方法里语句的顺序，那么“Done”被打印两次的几率会大大增加
  + 因为一个线程可能正在评估if，而另外一个线程在执行WriteLine语句，它还没来得及把done设为true。

# 锁定与线程安全 简介 Locking & Thread Safety
+ 在读取和写入共享数据的时候，通过使用一个互斥锁(exclusive lock)，就可以修复前面例子的问题。
+ C#使用lock语句来加锁
+ 当两个线程同时竞争一个锁的时候(锁可以基于任何引用类型对象)，一个线程会等待或阻塞，直到锁变成可用状态。
+ 在多线程上下文中，以这种方式避免不确定性的代码就叫做线程安全。
+ Lock不是线程安全的银弹，很容易忘记对字段加锁，lock也会引起一些问题(死锁)

# P5 向线程传递数据 & 异常处理
## 向线程传递数据
+ 如果你想往线程的启动方法里传递参数，最简单的方式是使用lambda表达式，在里面使用参数调用方法。(例子lambda)
+ 甚至可以把整个逻辑都放在lambda里面。(例子multi-lambda)
+ 在C#3.0之前，没有lamdba表达式。可以使用Thread的Start方法传递参数。(例子old-school)
+ Thread的重载构造函数可以接受下列两个委托之一作为参数：
  ···
  public delegate void ThreadStart();
  public delegate void ParameterizedThreadStart(object obj);
  ···

## Lamdba 表达式与被捕获的变量
+ 使用Lamdba表达式可以很简单的给Thread传递参数。但是线程开始后。可能会不小心修改了被捕获的变量，这要多加注意。(例子captured)

## 异常处理
+ 创建线程时在作用范围内的try/catch/finally块，在线程开始执行后就与线程无关了。
+ 在WPF、WinForm里，可以订阅全局一场处理事件：
  + Application.DispatcherUnhandledException
  + Application.ThreadException
  + 在通过消息循环调用的程序的任何部分发生未处理的异常(这相当于应用程序处于活动状态时在主线程上运行的所有代码)后，将触发这些异常。
  + 但是非 UI 线程上的未处理异常，并不会触发它。
+ 而任何线程有任何未处理的异常都会触发
  AppDomain.CurrentDomain.UnhandledException

# P6 前台和后台线程
## Foreground vs Background Threads 前台和后台线程
+ 默认情况下，你手动创建的线程就是前台线程。
+ 只要有前台线程在运行，那么应用程序就会一直处于活动状态。
  + 但是后台线程却不行。
  + 一旦所有的前台线程停止，那么应用程序就停止了
  + 任何的后台线程也会突然停止。
  
  + 注意：线程的前台、后台状态与它的优先级无关(所分配的执行时间)
+ 可以通过IsBackground属性判断线程是否后台线程。(例子background)
+ 进程以这种形式终止的时候，后台线程执行栈中的finally块就不会被执行了。
  + 如果想让它执行，可以在退出程序时使用Join来等待后台线程(如果是你自己创建的线程)，或者使用signal construct，如果线程池...

+ 应用程序无法正常退出的一个常见原因是还有活跃的前台线程。

# P7 线程优先级
+ 线程的优先级(Thread的Priority属性)决定了相对于操作系统中其它活跃线程所占的执行时间。
+ 优先级分为：
  + enum ThreadPriority { Lowest,BelowNormal,Normal,AboveNormal,Highest }

## 提升线程优先级
+ 提升线程优先级的时候需要特别注意，因为它可能“饿死”其它线程。
+ 如果想让某线程(Thread)的优先级比其他进程(Procee)中的线程(Thread)高，那么就必须提升进程(Process)的优先级
  + 使用System.Diagnostics下的Process类。
  ···
    using (Process p = Process.GetCurrentProcess())
        p.PriorityClass = ProcessPriorityClass.High;
  ···
+ 这可以很好地适用于只做少量工作且需要较低延迟的非UI进程。
+ 对于需要大量计算的应用程序(尤其是有UI的应用程序)，提高进程优先级可能会使其他进程饿死，从而降低整个计算机的速度。

# P8 信号简介
## 信号 Signaling
+ 有时，你需要让某线程一直处于等待的状态，直至接收到其它线程发来的通知。这就叫做singaling(发送信号)。
+ 最简单的信号结构就是ManualResetEvent。
  + 调用它上面的WaitOne方法会阻塞当前的线程，直到另一个线程通过调用Set方法来开启信号。
  + (例子Program17-signaling)
    + 调用完Set之后，信号会处于“打开”的状态。可以通过调用Reset方法将其再次关闭。
    

# P9 富客户端程序处理耗时操作的一种方法
## 富客户端应用程序的线程
+ 在WPF，UWP，WinForm等类型的程序中，如果在主线程执行耗时的操作，就会导致整个程序无响应。因为主线程同时还需要处理消息循环，而渲染和鼠标键盘事件处理等工作都是消息循环来执行的。
+ 针对这种耗时的操作，一种流行的做法是启用一个worker线程。
  + 执行完操作后，再更新到UI
+ 富客户端应用的线程模型通常是：
  + UI元素和控件只能从创建它们的线程来进行访问(通常是主UI线程)
+ 比较底层的实现是：
  + WPF，在元素的Dispacther对象上调用BeginInovke或Invoke。
  + WinForm，调用空间的BeginInovke或Inovke。
  + UWP，调用Dispatcher对象上的RunAsync或Inovke。
+ 所有这些方法都接收一个委托。
+ BeginInovke或RunAsync通过将委托排队到UI线程的消息队列来执行工作。
+ Invoke执行相同的操作，但随后会进行阻塞，直到UI线程读取并处理消息。
  + 因此，Inovke允许您从方法中获取返回值。
  + 如果不需要返回值，BeginInovke/RunAsync更可取，因为他们不会阻塞调用方，也不会引入死锁的可能性

# P10 Synchronization Contexts
## Synchronization Contexts 同步上下文
+ 在System.ComponmentModel下有一个抽象类：SynchronizationContext，它使得Thread Marshaling得到泛化。
+ 针对移动、桌面(WPF，UWP，WinForm)等富客户端应用的API，他们都定义和实例化了SynchronizationContext的子类
  + 可以通过静态属性SynchronizationContext.Current来获得(当运行在UI线程时)
  + 捕获该属性让你可以在稍后的时候从worker线程向UI线程发送数据(例子)
  + 调用Post就相当于调用Dispatch或Control上面的BeginInovke方法
  + 还有一个Send方法，它等价于Inovke方法

# P11 线程池
## 线程池 Thread Pool
+ 当开始一个线程的时候，将花费几百微妙来组织类似以下的内容：
  + 一个新的局部变量栈
+ 线程池就可以节省这种开销
  + 通过预先创建一个可循环使用线程的池来减少这一开销。
+ 线程池对于高效的并行编程和细粒度开发是必不可少的
+ 它允许不被线程启动的开销淹没的情况下运行短期操作

## 使用线程池线程需要注意的几点
+ 不可以设置池线程的Name
+ 池线程都是后台线程
+ 阻塞池线程可使性能降低

+ 你可以自由的更改池线程的优先级
  + 当它释放回池的时候优先级将还原为正常状态
+ 可以通过Thread.CurrentThread.IsThreadPoolThread属性来判断是否

## 进入线程池
+ 最简单的、显示的在线程池运行代码的方式就是使用Task.Run
```
// Task is in System.Threading.Tasks
Task.Run(() => Console.WriteLine("Hello from the thread pool"));
```

## 谁使用了线程池
+ WCF、Remoting、ASP.NET、ASMX Web Services应用服务器
+ System.Timers.Timer、System.Threading.Timer
+ 并行编程结构
+ BackgroundWorker类(现在很多余)
+ 异步委托(现在很多余)

## 线程池中的整洁
+ 线程池提供了另一个功能，即确保临时超出计算-Bound的工作不会导致CPU超额订阅
+ CPU超额订阅：活跃的线程超过CPU的核数，操作系统就需要对线程进行时间切片
+ 超额订阅对性能影响很大，时间切片需要昂贵的上下文切换，并且可能使CPU缓存失效，而CPU缓存对现代处理器的性能至关重要

## 线程池中的整洁 CLR的策略
+ CLR通过对任务排队并对其启动进行节流限制来避免线程池中的超额订阅。
+ 它首先运行尽可能多的并发任务(只要还有CPU核)，然后通过爬山算法调整并发级别，并在特定方向上不断调整工作负载。
  + 如果吞吐量提高，它将继续朝同一方向(否则将反转)。
+ 这确保它始终追随最佳性能曲线，即使面对计算机上竞争的进程活动时也如此
+ 如果下面两点能够满足，那么CLR的策略将发挥出最佳效果：
  + 工作项大多是短时间运行的(<250毫秒，或者理想情况下<100毫秒)，因此CLR有很多机会进行测量和调整。
  + 大部分时间都被阻塞的工作项不会主宰线程池

# P12 开始一个Task
## Thread的问题
+ 线程(Thread)是用来创建并发(concurrency)的一种低级别工具，它有一些限制，尤其是：
  + 虽然开始线程的时候可以方便的传入数据，但是当Join的时候，很难从线程获得返回值。
    + 可能需要设置一些共享字段。
    + 如果操作抛出异常，捕获和传播该异常都很麻烦。
  + 无法告诉线程在结束时开始做另外的工作，你必须进行Join操作(在进程中阻塞当前的线程)
+ 很难使用较小的并发(concurrent)来组建大型的并发
+ 导致了对手动同步的更大依赖以及随之而来的问题

## Task Class
+ Task类可以很好的解决上述问题
+ Task是一个相对高级的抽象：它代表了一个并发操作(concurrent)
  + 该操作可能由Thread支持，或不由Thread支持
+ Task可以使用线程池来减少启动延迟
+ 使用TaskCompletionSource，Task可以利用回调的方式吗，在等待I/O绑定操作时完全避免线程。

## 开始一个Task Task.Run
+ Task类在System.Threading.Tasks命名空间下。
+ 开始一个Task最简单的办法就是使用Task.Run(.NET 4.5, 4.0的时候是Task.Factory.StartNew)这个静态方法：
  + 传入一个Action委托即可(例子task)
+ Task默认使用线程池，也就是后台线程：
  + 当主线程结束时，你创建的所有tasks都会结束。(例子task)
+ Task.Run返回一个Task对象，可以使用它来监视其过程
  + 在Task.Run之后，我们没有调用Start，因为该方法创建的时“热”任务(hot task)
    + 可以通过Task的构造函数创建“冷”任务(cold task)，但是很少这样做
  + 可以通过Task的Status属性来跟踪task的执行状态。

## Wait 等待
+ 调用task.Wait方法会进行阻塞直到操作完成
  + 相当于调用thread上的Join方法
  + (例子wait)
+ Wait也可以让你指定一个超时时间和一个取消令牌提前结束等待。

## Long-runnning tasks 长时间运行的任务
+ 默认情况下，CLR在线程池中运行Task，这非常适合短时间运行的Compute-Bound类工作。
+ 针对长时间运行的任务或者阻塞操作(例如前面的例子)，你可以不采用线程池(例子longRunning)
+ 如果同时运行多个long-running tasks(尤其是其中有处于阻塞状态的)，那么性能会受很大影响，这时有比TaskCreationOptions.LongRunning更好的办法：
  + 如果任务是IO-Bounds，TaskCompletionSource和异步函数可以让你用回调(Coninuations)代替线程来实现并发。
  + 如果任务是Compute-Bound，生产者/消费者队列允许你对任务的并发性进行限流，避免把其他线程和进程饿死。

# P13 Task的返回值
+ Task有一个泛型子类叫做Task<TResult>，它允许发出一个返回值。
+ 使用Func<TResult>委托或兼容的Lambda表达式来调用Task.Run就可以得到Task<TResult>。
+ 随后，可以通过Result属性来获得返回的结果。
  + 如果这个task还没有完成操作，访问Result属性会阻塞该线程直到该task完成操作。
  + (例子tresult, prime)
+ Task<TResult>可以看作是一种所谓的“未来许诺”(future、promise)，在它里面包裹着一个Result，在稍后的时候就会变得可用。
+ 在CTP版本的时候，Task<TResult>实际上叫做Future<TResult>

# P14 Task的异常
+ 与Thread不一样，Task可以很方便的传播异常
  + 如果你的task里面抛出了一个未处理的异常(故障)，那么该异常就会重新被抛出给：
    + 调用了wait()的地方
    + 访问了Task<TResult>的Result属性的地方。
    + (例子exception)
+ CLR将异常包裹在AggregateException里，以便在并行编程场景中发挥良好的作用。
+ 无需抛出异常，通过Task的IsFaulted和IsCanceled属性也可以检测出Task是否发生了故障；
  + 如果两个属性都返回false，那么没有错误发生。
  + 如果IsCanceled为true，那么说明一个OperationCanceledException为该Task抛出了。
  + 如果IsFaulted为true，那就说明另一个类型的异常被抛出了，而Exception属性也将指明错误。

## 异常与“自治”的Task
+ 自治的，“设置完就不管了”的Task。就是指不通过调用Wait()方法、Result属性或continuation进行会合的任务。
+ 针对自治的Task，需要像Thread一样，显式的处理异常，避免发生“悄无声息的故障”。
+ 自治Task上未处理的异常为未观察到的异常。

## 未观察到的异常
+ 可以通过全局的TaskScheduler.UnobservedTaskException来订阅未观察到的异常。
+ 关于什么是“未观察到的异常”，有一些细微的差别：
  + 使用超时进行等待的Task，如果在超时后发生故障，那么它将会产生一个“未观察到的异常”。
  + 在Task发生故障后，如果访问Task的Exception属性，那么该异常就被认为是“已观察到的”。

# P15 Continuation 继续/延续
+ 一个Continuation会对Task说：“当你结束的时候，继续再做点其它的事”
+ Continuation通常事通过回调的方式实现的
  + 当操作一结束，就开始执行
  + (例子prime)
    + 在Task上调用GetAwaiter会返沪一个awaiter对象
      + 它的OnCompleted方法会告诉之前的task，“当你结束/发生故障的时候要执行委托”
    + 可以将Continuation附加到已经结束的task上面，此时Continuation将会被安排立即执行。

## awaiter
+ 任何可以暴露下列两个方法和一个属性的对象就是awaiter：
  + OnCompleted
  + GetResult
  + 一个叫做IsCompleted的bool属性
+ 没有接口或者父类来统一这些成员。
+ 其中OnCompleted是INotifyCompletion的一部分

## 如果发生故障
+ 如果之前的任务发生故障，那么当Continuation代码调用awaiter.GetResult()的时候，异常就会被重新抛出。
+ 无需调用GetResult，我们可以直接访问task的Result属性。
+ 但调用GetResult的好处是，如果task发生故障，那么异常就会被直接的抛出，而不是包裹在AggregateException里面，这样的话catch块就会简洁很多了。

## 非泛型task
+ 针对非泛型的task，GetResult()方法有一个返回值，它就是用来重新抛出异常。

## 同步上下文
+ 如果同步上下文出现了，那么OnCompleted会自动捕获它，并将Continuation提交到这个上下文中。这一点在富客户端应用中非常有用，因为它会把Continuation放回到UI线程中。
+ 如果是编写一个库，则不希望出现上述行为，因为开销较大的UI线程切换应该在程序运行离开库的时候只发生一次，而不是出现在方法调用之间。所以，我们可以使用ConfigureAwait方法来避免这种行为(例子configureAwait)
+ 如果没有同步上下文出现，或者你使用的是ConfigureAwait(false)，那么Continuation会运行在先前task的同一个线程上，从而避免不必要的开销

## ContinueWith
+ 另一种附加Continuation的方式就是调用task的ContinueWith方法(例子continueWith)
+ ContinueWith本身返回一个task，它可以用它来附加更多的Continuation。
+ 但是，必须直接处理AggregateException：
  + 如果task发生故障，需要写额外的代码来把Continuation封装(marshal)到UI应用上。
  + 在非UI上下文中，若想让Continuation和task执行在同一个线程上，必须指定TaskContinuationOptions.ExecuteSynchronously，否则它将弹回到线程池。
+ ContinueWith对于并行编程来说非常有用。

# P16 TaskCompletionSource
+ Task.Run创建Task
+ 另一种方式就是用TaskCompletionSource来创建Task
+ TaskCompletionSource让你在稍后开始和结束的任意操作中创建Task
  + 它会为你提供一个可手动执行的“从属”Task
    + 指示操作何时结束或发生故障
+ 它对IO-Bound类工作比较理想
  + 可以获得所有Task的好处(传播值、异常、Continuation等)
  + 不需要在操作时阻塞线程

## 使用TaskCompletionSource
+ 初始化一个实例即可
+ 它有一个Task属性可以返回一个Task
+ 该Task完全由TaskCompletionSource对象控制
```
public class TaskCompletionSource<Task>
{
    public void SetResult(TResult result);
    public void SetException(Exception exception);
    public void SetCanceled();

    public void TrySetResult(TResult result);
    public void TrySetException(Exception exception);
    public void TrySetCanceled();
    public void TrySetCanceled(CancellationToken cancellationToken);
    ...
}
```
+ 调用任意一个方法都会Task给发信号：
  + 完成、故障、取消
+ 这些方法只能调用一次，如果再次调用：
  + SetXxx会抛出异常
  + TryXxx会返回false
  + (例子tcs)
  + (例子run)

## TaskCompletionSource的真正魔力
+ 它创建Task，但并不占用线程
+ (例子timer)
+ (例子delay)

## Task,Delay
+ (例子master)

# P17 同步和异步
## 同步 vs 异步
+ 同步操作会在返回调用者之前完成它的工作
+ 异步操作会在返回调用者之后去做它的(大部分)工作
  + 异步的方法更为少见，会启用并发，因为他的工作会与调用者并发执行
  + 异步方法通常很快(立即)就会返回到调用者，所以叫非阻塞方法
+ 目前见到的大部分的异步方法都是通用目的的：
  + Threaad.Start
  + Task.Run
  + 可以将continuation附加到的Task的方法

## 什么是异步编程
+ 异步编程的原则是将长时间运行的函数写成异步的。
+ 传统的做法是将长时间运行的函数写成同步的，然后从新的线程或Task进行调用，从而按需引入并发。
+ 上述异步方式的不同之处在于，它是从长时间运行函数的内部启动并发。这有两点好处：
  + IO-bound并发可不适用线程来实现。可提高可扩展性和执行效率；
  + 富客户端再worker线程会使用更少的代码，简化了线程安全性。

## 异步编程的两中用途
+ 编写高效处理大量并发IO的应用程序(典型的：服务器端应用)
  + 挑战并不是线程安全(因为共享状态通常是最小化的)，而是执行效率
    + 特别的，每个网络请求并不会消耗一个线程
+ 调用图(call graph)
+ 在富客户端应用里简化线程安全。
  + 如果调用图中任何一个操作是长时间运行的。那么整个call graph必须运行在worker线程上，以保证UI的响应。
    + 得到一个横跨多个方法的单一并发操作(粗粒度)；
    + 需要为call graph中的每个方法考虑线程安全。
  + 异步的call graph，直到需要钱开启一个线程，通常较浅(IO-bound操作完圈不需要)

## 经验之谈
+ 为了获得上述好处，下列操作建议异步编写：
  + IO-bound和Compute-bound操作
  + 执行超过50毫秒的操作
+ 另一方面过细的粒度会损害性能，因为异步操作也有开销。

# P18 异步和Continuation以及语言的支持
## 异步编程和Continuation
+ Task非常适合异步编程，因为他们支持Continuation(它对异步非常重要)
  + 第16讲里面TaskCompletionSource的例子
  + TaskCompletionSource是实现底层IO-bound异步方法的一种标准方式
+ 对于Compute-bound方法，Task.Run会初始化绑定线程的并发
  + 把task返回调用者，创建异步方法；
  + 异步编程的区别：目标是再调用图较低的位置来这样做。
    + 富客户端应用中，高级方法可以保留再UI线程和访问控制以及共享状态上，不会出现线程安全问题

## 语言对异步的支持非常重要
+ (例子Program31)
+ 需要对task的执行序列化
  + 例如Task B依赖于Task A的执行结果
  + (例子Program32)为此，必须在continuation内部触发下一次循环
+ async和await
  + 对于不想复杂的实现异步非常重要。
+ 命令式循环结构不要和continuation混合在一起，因为它们依赖于当前本地状态。
+ 另一种实现，函数式写法(Linq查询)，它也是响应式编程(Rx)的基础。

# P19 await
## 异步函数
+ async和await关键字可以让你写出和同步代码一样简洁且结构相同的异步代码

## awaiting
+ await关键字简化了附加continuation的过程。
+ 其结构如下：
```
var result = await expression;
statement(s);
```
+ 它的作用相当于：
```
var awaiter = expression.GetAwaiter();
awaiter.OnCompleted(() =>
{
    var result = awaiter.GetResult();
    statement(s);
});
```

## async修饰符
+ async修饰符会让编译器把await当作关键字而不是标识符(C# 5以前可能会使用await作为标识符)
+ async修饰符只能应用于方法(包括lambda表达式)。
  + 该方法可以返回void、Task、Task<TResult>
+ async修饰符对方法的签名或public元数据没有影响(和unsafe一样)，只会影响方法内部。
  + 在接口内使用async是没有意义的
  + 使用async来重载非async的方法却是合法的(只要方法签名一致)
+ 使用了async修饰符的方法就是“异步函数”。


## 异步方法的执行
+ 遇到await表达式，执行(正常情况下)会返回调用者
  + 就像iterator里面的yield return。
  + 在返回前，运行时会附加一个continuation到await的task
    + 为保证task结束时，执行会跳回原方法，从停止的地方继续执行。
  + 如果发生故障，那么异常就会被重新抛出
  + 如果一切正常，那么它的返回值就会赋给await表达式
  + (例子Program35)

## 可以await什么?
+ 你await的表达式通常时一个
+ 也可以满足下列条件的任意对象
  + 有GetAwaiter方法，它返回一个awaiter(实现了INotifyCompletion.OnCompleted接口)
  + 返回适当类型的GetResult方法
  + 一个bool类型的IsCompleted属性

## 捕获本地状态
+ await表达式的最牛之处就是它几乎可以出现在任何地方。
+ 特别的，在异步方法内，await表达式可以替换任何表达式。
  + 除了lock表达式和unsafe上下文
+ (例子Program36)

## await之后在哪个线程上执行
+ 在await表达式之后，编译器依赖于continuation(通过awaiter模式)来继续执行
+ 如果在富客户端应用的UI线程上，同步上下文保证后续是在原线程上执行；
+ 否则，就会在task结束的线程上继续执行。

## UI上的await
+ (例子Program37)
+ 本例中，只有GetPrimesCountAsync中的代码在worker线程上运行
+ Go中的代码会”租用“UI线程上的时间
+ 可以说：Go是在消息循环中“伪并发”的执行
  + 也就是说：它和UI线程处理的其它时间是穿插执行的
  + 因为这种伪并发，唯一能发生“抢占”的时刻就是在await期间。
    + 这其实简化了线程安全，防止重新进入即可
+ 这种并发发生在调用栈较浅的地方(Task.Run调用的代码里)
+ 为了从该模型获益，真正的并发代码就要避免访问共享状态或者UI控件。


## UI上的await
+ (例子)
+ 伪代码：
```
为本线程设置同步上下文(WPF)
while(!程序结束)
{
    等着消息队列发生一些事情
    发生了事情，是哪种消息?
    键盘/鼠标消息->触发 event handler
    用户BeginInvoke/Invoke 消息->执行委托
}
```

## 与粗粒度的并发比较
+ 例如使用BackgroundWorker(例子，Task.Run)
+ 整个同步调用图都在worker线程上
+ 必须在代码中到处使用Dispatcher.BeginInvoke
+ 循环本身在worker线程上
+ 引入了race condition
+ 若实现取消和过程报告，会使得线程安全问题更容易发生，在方法中添加任何的代码也是同样的效果


# P20 编写异步函数
## 编写异步函数
+ 对于任何异步函数，你可以使用Task替代void作为返回类型，让该方法成为更有效的异步(可以进行await)。【例子611-Program38】
+ 并不需要在方法体中显示的返回Task。编译器会生成一个Task(当方法完成或发生异常时)，这使得创建异步的调用链非常方便【例子612-Program39】
+ 编译器会对返回Task的异步函数进行扩展，使其成为当发送信号或发生故障时使用TaskCompletionSource来创建Task的代码。【大致代码：例子6121-Program40】
+ 因此，当返回Task的异步方法结束的时候，执行就会跳回对它进行await的地方。(通过continuation)


## 富客户端场景下
+ 富客户端场景下，执行在此刻会跳回到UI线程(如果目前不在UI线程的话)。
+ 否则，就在continuation返回的任意线程上继续执行。
+ 这意味着，在异步调用图中向上冒泡的时候，不会发生延迟成本，除非是UI线程启动的第一次“反弹”。


## 返回Task<TResult>
+ 如果方法体返回TResult，那么异步方法就可以返回Task<TResult>。【例子6131-Program41】
+ 其原理就是给TaskCompletionSource发送的信号带有值，而不是null。【例子6132-Program42】
+ 与同步编程很相似，是故意这样设计的。【同步版本：例子6133-Program43】

## C#中如何设计异步函数
+ 以同步的方式编写方法
+ 使用异步调用来代替同步调用，并且进行await
+ 除了顶层方法外(UI控件的event handler)，把你方法的返回类型升级为Task或Task<TResult>，这样它们就可以进行await了。

## 编译器能对异步函数生成Task意味着什么？
+ 大多数情况下，你只需要在初始化IO-bound并发的底层方法里显式的初始化TaskCompletionSource，这种情况很少见。
+ 针对初始化compute-bound的并发方法，你可以使用Task.Run来创建Task。


## 异步调用图执行
+ 【例子614-Program44】
+ 整个执行与之前同步例子中调用图执行的顺序一样，因为我们对每个异步函数的调用都进行了await。
+ 在调用图中创建了一个没有并行和重叠的连续流。
+ 每个await在执行中都创建了一个间隙，在间隙后，程序可以从中断处恢复执行。

## 并行(Parallelism)
+ 不使用await来调用异步函数会导致并行执行的发生。
+ 例如：-button.Click += (sender,args) => Go();
  + 确实也能妈祖保持UI响应的并发要求。
+ 同样，可以并行跑两个操作：
```
var task1 = PrintAnswerToLife();
var task2 = PrintAnswerToLife();
await task1;
await task2;
```

## 异步Lambda表达式
+ 匿名方法(包括Lambda表达式)，通常使用async也可以变成异步方法。
+ 调用方式也一样。【例子616-Program45】
+ 附加event handler的时候也可以使用异步Lambda表达式【例子6161】
```
myButton.Click += async (sender, args) =>
{
    await Task.Delay(1000);
    myButton.Content = "Done";
};
```
+ 也可以返回Task<TResult>。【例子6162-Program46】

# P21 异步和同步上下文(synchornization contexts)
## 发布异常
+ 富客户端引用通常依赖于集中的异常处理事件来处理UI线程上未捕获的异常。
  + 例如WPF中的Application.DispatcherUnhandledException
  + ASP.NET Core中定制ExceptionFilterAttribute也是差不多的效果
+ 其内部原理就是：通过在它们自己的try/catch块来调用UI事件(在ASP.NET Core里就是页面处理方法的管道)
+ 顶层的异步方法会使事情更加复杂。
```
async void ButtonClick(object sender, RoutedEventArgs args)
{
    await Task.Delay(1000);
    throw new Exception("Will the be ignored?")
}
```
+ 当点击按钮，event handler运行时，在await后，执行会正常的返回到消息循环；1秒钟之后会抛出的异常无法被消息循环中的catch捕获。
+ 为了缓解该问题，AsyncVoidMethodBuilder会捕获未处理的异常(在返回void的异步方法里)，并把它们发布到同步上下文(如果出现的话)，以确保全局处理事件能够触发。

## 注意
+ 编译器只会把上述逻辑应用于返回类型为void的异步方法。
+ 如果ButtonClick的返回类型是Task，那么未处理的异常将导致结果Task出错，然后Task无处可去(导致未观察到的异常)

## 发布异常
+ 一个有趣的细微区别：无论你在await前面还是后面抛出异常，都没有区别。
+ 因此，下例中，异常会被发布到同步上下文(如果出现的话)，而不会发布给调用者。
  + async void Foo() { throw null; await Task.Delay(1000); }
  + 如果同步上下文没有出现，异常将会在线程池上传播，从而终止应用程序。
+ 不直接将异常抛回调用者的原因是为了确保可预测性和一致性。
+ 在下例中，不管someCondition是什么值，InvalidOperationException将始终得到和导致Task出错同样的效果
```
async Task Foo()
{
    if (someCondition) await Task.Delay(100);
    throw new InvalidOperationException();
}
```
+ Iterator也是一样的：`IEnumerable<int> Foo() { throw null; yield return 123; }`
  + 本例中，异常绝不会直接返回给调用者，直到序列被遍历后，才会抛出异常。

## OperationStarted 和 OperationCompleted
+ 如果存在同步上下文，返回void的异步函数也会在进入函数时调用其OperationStarted方法，在函数完成时调用其OperationCompleted方法
+ 如果为了对返回void的异步方法进行单元测试而编写一个自定义的同步上下文，那么重写这两个方法确实有用。

# P22 优化：同步完成
+ 异步函数可以在await之前就返回。【例子-Program47】
+ 如果URI在缓存中存在，那么不会有await发生，执行就会返回给调用者，方法会返回一个已经设置信号的Task，这就是同步完成。
+ 当await同步完成的Task时，执行不会返回到调用者，也不会通过continuation跳回。它会立即执行到下个语句。

+ 编译器时通过检查awaiter上的IsCompleted属性来实现这个优化的。也就是说，无论何时，当你await的时候：
`Console.WriteLine(await GetWebPageAsync("http://oreilly.com"));`
+ 如果是同步完成，编译器会释放可短路continuation的代码。
```
var awaiter = GetWebPageAsync().GetWaiter();
if (awaiter.IsCompleted)
    Console.WriteLine(awaiter.GetResult());
else
    awaiter.OnCompleted(() => Console.WriteLine(awaiter.GetResult()));
```

## 注意
+ 对一个同步返回的异步方法进行await，仍然会引起一个小的开销(20纳秒左右，2019年的PC)
+ 反过来，跳回到线程池，会引入上下文切换开销，可能是1-2毫秒
+ 而跳回到UI的消息循环，至少是10倍开销(如果UI繁忙，那时间更长)

+ 编写完全没有await的异步方法也是合法的，但是编译器会发出警告：
`async Task<string> Foo() { return "abc"; }`
+ 但这类方法可以用于重载virtual/abstract方法。
+ 另外一种可以达到相同结果的方式是：使用Task.FromResult，它会返回一个已经设置好信号的Task。
`Task<string> Foo() { return Task.FromResult("abc"); }`

+ 如果从UI线程调用，那么GetWebPageAsync方法是隐式线程安全的。您可以连续多次调用它(从而启动多个并发下载)，并且不需要lock来保护缓存。
+ 有一种简单的方法可以实现这一点，而不必求助于lock或信令结构。我们创建一个“futures”(Task<string>)的缓存，而不是字符串的缓存。注意并没有async;
```
static Dictonary<string,Task<string>> _cache = new Dictonary<string,Task<string>>();

Task<string> GetWebPageAsync(string uri)
{
    if (_cache.TryGetValue(uri, out downloadTask)) return downloadTask;
    return _cache[uri] = new WebClient().DownloadStringTaskAsync(uri);
}
```

## 不使用同步上下文，使用lock也可以
+ lock的不是下载的过程，lock的是检查缓存的过程(很短暂)
```
lock (_cache)
{
    if (_cache.Trygetvalue(uri, out var downloadTask))
        return downloadTask;
    else
        return _cache[uri] = new WebClient().DwonloadStrigAsync(uri);
}
```

# P23 ValueTask<T>
+ ValueTask<T>用于微优化场景，您可能永远不需要编写返回此类型的方法。
+ Task<T>和Task是引用类型，实例化它们需要基于堆的内存分配和后续的收集。
+ 优化的一种极端形式是编写无需分配的此类内存的代码；换句话说，这不会实例化任何引用类型，不会给垃圾收集增加负担。
+ 为了支持这种模式，C#引入了ValueTask和ValueTask<T>这两个struct，编译器允许使用它们代替Task和Task<T>
  + `async ValueTask<int> Foo() {...}`

+ 如果操作是同步完成的，则await ValueTask<T>是无分配的。
  + `int answer = await Foo(); // (可能是)无分配的`
+ 如果操作不是同步完成的，ValueTask<T>实际上就会创建一个普通的Task<T>(并将await转发给它)
+ 使用AsTask方法，可以把ValueTask<T>转化为Task<T>(也包括非泛型版本)

## 使用ValueTask<T>时的注意事项
+ ValueTask<T>并不常见，它的出现纯粹是为了性能。
+ 这意味着它被不恰当的值类型语义锁困扰，这可能会导致意外。为避免错误行为，必须避免以下情况：
  + 多次await同一个ValueTask<T>
  + 操作没结束的时候就调用.GetAwaiter().GetResult()
+ 如果你需要进行这些操作，那么先调用AsTask方法，操作它返回的Task。

+ 避免上述陷阱最简单的办法就是直接await方法调用：
  + `await Foo();`
+ 将ValueTask赋给变量时，就可能引发错误了：
  + `ValueTask<T> valueTask = Foo();`
+ 将其立即转化为普通的Task，就可以避免此类错误的发生：
  + `Task<int> task = Foo().AsTask();`

## 避免过度的弹回
+ 对于在循环中多次调用的方法，通过调用ConfigureAwait方法，就可以避免重复的弹回到UI消息循环所带来的开销。
+ 这强迫Task不把continuation弹回给同步上下文。从而将开销削减到接近上下文切换的成本(如果您await的方法同步完成，则开销会小得多)：
```
async void A() {...await B();...}

async Task B()
{
    for (int i = 0; i < 1000; i++)
        await C().ConfigureAwait(false);
}

async Task C() { ... }
```
  + 这意味着对于方法B和C，我们取消了UI程序中的简单线程安全模型，即代码在UI线程上运行，并且只能在await语句期间被抢占。但是，方法A不受影响，如果在一个UI线程上启动，它将保留在UI线程上。
  + 这种优化在编写库时特别重要：您不需要简化线程安全性带来的好处，因为您的代码通常不与调用方共享状态，也不访问UI控件。

# P24 取消
## 取消cancellation
+ 使用取消标志对并发进行取消，可以封装一个类
```
class CancellationToken
{
    public bool IsCancellationRequested { get; private set; }
    public void Cancel () { IsCancellationRequested = true; }
    public void ThrowIfCancellationRequested()
    {
        if (IsCancellationRequested)
            throw new OperationCanceledException();
    }
}
```

```
async Task Foo (CancellationToken cancellationToken)
{
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine(i);
        await Task.Delay(1000);
        cancellationToken.ThrowIfCancellationRequested();
    }
}
```
  + 当调用者想取消的时候，它调用CancellationToken上的Cancel方法。这就会把IsCancellationRequested设置为true，即会导致短时间后Foo会通过OperationCanceledException引发错误。


## CancellationToken和CancellationTokenSource
+ 先不管线程安全(应该在读写IsCancellationRequested时进行lock)，这个模式非常有效，CLR也提供了一个CancellationToken类，它的功能和前面的例子类似。
+ 但是它缺少一个Cancel方法，Cancel方法在另外一个类上进行暴露：CancellationTokenSource
+ 这种分离的设计时出于安全考虑：只能对CancellationToken访问的方法可以检查取消，但不能实例化取消。

## 获取CancellationToken
+ 想获得取消标志(cancellation token)，先实例化CancellationTokenSource：
  + `var cancelSource = new CancellationTokenSource();`
+ 这会暴露一个Token属性，它会返回一个CancellationToken，所以我们可以这样调用：
```
var cancelSource = new CancellationTokenSource();
Task foo = Foo(cancelSource.Token);
...
... (some time later)
cancelSource.Cancel();
```

## Delay
+ CLR里大部分的异步方法都支持CancellationToken，包括Delay方法。
```
async Task Foo (CancellationToken cancellationToken)
{
    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine(i);
        await Task.Delay(1000, cancellationToken);
    }
}
```
+ 这时，task在遇到请求时会立即停止(而不是1秒钟之后才停止)
+ 这里，我们无需调用ThrowIfCancellationRequested，因为Delay会替我们做。
  + 取消标记在调用栈中很好地向下传播(就像是因为异常，取消请求在调用栈中向上级联一样)。

## 同步方法
+ 同步方法也支持取消(例如Task的Wait方法)。这种情况下，取消指令需要异步发出(例如来自另一个Task)
```
var cancelSource = new CancellationTokenSource();
Task.Delay(5000).ContinueWith(ant => cancelSource.Cancel());
```

## 其它
+ 事实上，你可以在构造CancellationTokenSource时指定一个时间间隔，以便在一段时间后启动取消。它对于实现超时非常有用，无论时同步还是异步：
```
var cancelSource = new CancellationTokenSource(5000);
try { await Foo(cancelSource.Token); }
catch (OperationCanceledException ex) { Console.WriteLine("Cancelled"); }
```
+ CancellationToken这个struct提供了一个Register方法，它可以让你注册一个回调委托，这个委托会在取消时触发。它会返回一个对象，这个对象在取消注册时可以被Dispose掉。

+ 编译器的异步函数生成的Task在遇到未处理的OperationCanceledException异常时会自动进入取消状态(IsCanceled返回true，IsFaulted返回false)
+ 使用Task.Run创建的Task也是如此。这里是指向构造函数传递(相同的)CancellationToken。
+ 在异步场景中，故障Task和取消的Task之间的区别并不重要，因为它们在await时都会抛出一个OperationCanceledExeption。但这在高级并行变成场景(特别是条件continuation)中很重要。

# P25 进度报告
+ 有时，你希望异步操作在运行的过程中能实时的反馈进度。一个简单的解决办法是向异步方法中传入一个Action委托，当进度变化的时候触发方法调用：
```
Task Foo(Action<int> onProgressPercentChanged)
{
    return Task.Run(() =>
    {
        for (int i = 0; i < 1000; i++)
        {
            if (i % 10 == 0) onProgressPercentChanged(i / 10);
            // Do somthing compute-bound...
        }
    });
}

Action<int> progress = i => Console.WriteLine(i + " %");
await Foo(progress);
```
   + 尽管这段代码可以在Console App中很好的应用，但在富客户端应用中却不理想。因为它是从worker线程报告的进度，可能会导致消费者的线程安全问题。

## IPROGRESS<T> 和 PROGRESS<T>
+ CLR提供了一对类型来解决此问题：
  + IPROGRESS<T>接口
  + PROGRESS<T>类(实现了上面的接口)
+ 它们的目的就是包装一个委托，以便UI线程可以安全的通过同步上下文来报告进度。
+ 接口定义如下：
```
public interface IProgress<in T>
{
    void Report(T value);
}
```
+ 使用IPROGRESS<T>：
```
Task Foo(IProgress<int> onProgressPercentChanged)
{
    return Task.Run(() =>
    {
        for (int i = 0; i < 1000; i++)
        {
            if (i % 10 == 0) onProgressPercentChanged.Report(i / 10);
            // Do something compute-bound
        }
    });
}
```
+ Progress<T>的一个构造函数可以接受Action<T>类型的委托：
```
var progress = new Progress<int> (i => Console.WriteLine(i + " %"));
await Foo(progress);
```
+ Progress<T>还有一个ProgressChanged事件，您可以订阅它，而不是[或附加的]将Action委托传递给构造函数。
+ 在实例化Progress<T>时，类捕获同步上下文(如果存在)。
  + 当Foo调用Report时，委托时通过该上下文调用的。
+ 异步方法可以通过将int替换为公开一系列属性的自定义类型来实现更精细的进度报告。


# P26 TAP
## Task-Based Asynchronous Pattern
+ .Net Core暴露了数百个返回Task且可以await的异步方法(主要和I/O相关)。大多数方法都遵循一个模式，叫做基于Task的异步模式(TAP)。这是我们迄今为止所描述的合理形式化。TAP方法执行以下操作：
  + 返回一个“热”(运行中的)Task或Task<TResult>
  + 方法名以Async结尾(除了像Task组合器等情况)
  + 会被重载，以便接受CancellationToken或(和)IProgress<T>，如果支持相关操作的话。
  + 快速返回调用者(只有很小的初始化同步阶段)
  + 如果时I/O绑定，那么无需绑定线程

# P27 Task组合器
+ 异步函数有一个让其保持一致的协议(可以一致的返回Task)，这能让其保持良好的结果：可以使用以及编写Task组合器，也就是可以组合Task，但是并不关心Task具体做什么的函数。
+ CLR提供了两个Task组合器
  + Task.WhenAny
  + Task.WhenAll
+ 本节课假设定义了以下方法：
```
async Task<int> Delay1() { await Task.Delay (1000); return 1; }
async Task<int> Delay2() { await Task.Delay (2000); return 2; }
async Task<int> Delay3() { await Task.Delay (3000); return 3; }
```

## WhenAny
+ 当一组Task中任何一个Task完成时，Task.WhenAny会返回完成的Task。
```
Task<int> winningTask = await Task.WhenAny(Delay1(), Delay2(), Delay3());
Console.WriteLine("Done");
Console.WriteLine(winningTask.Result);  // 1
```
+ 因为Task.WhenAny本身就返回一个Task，我们对它进行await，就会返回最先完成的Task。
+ 上例完全时非阻塞的，包括最后一行(当访问Result属性时，winningTask已完成)，但最好还是对winningTask进行await，因为异常无需AggregateException包装就会重新抛出：`Console.WriteLine(await winningTask);  // 1`
+ 实际上，我们可以在一步中执行两个await：`int answer = await await Task.WhenAny(Delay1(), Delay2(), Delay3());`
+ 如果“没赢”的Task后续发生了错误，那么异常将不会被观察到，除非你后续对它们进行await(或者查询其Exception属性)
+ WhenAny很适合为不支持超时或取消的操作添加这些功能：
```
Task<string> task = SomeAsyncFunc();
Task winner = await (Task.WhenAny(task, Task.Delay(5000)));
if (winner != task) throw new TimeoutException();
string result = await task;  // Unwrap result/re-throw
```
+ 注意：本例中返回的结果是Task类型。


## WhenAll
+ 当传给它的所有的Task都完成后，Task.WhenAll会返回一个Task。
  + `await Task.WhenAll(Delay1(), Delay2(), Delay3());`
  + 本例就会在3秒后结束。
+ 通过轮流对3个Task进行await，也可以得到类似的结果：
```
Task task1 = Delay1(), task2 = Delay2(), task3 = Delay3();
await task1; await task2; await task3;
```
  + 不同点是(除了3个await的低效)：如果task1出错，我们就无需等待task2和task3了，它们的错误也不会被观察到。

## WhenAll异常
+ 与之相对，Task.WhenAll直到所有Task完成，它才会完成，即使有错误发生。如果有多个错误，它们的异常会包裹在Task的AggregateException里
+ await组合的Task，只会抛出第一个异常，想要看到所有的异常，你需要这样做：
```
Task task1 = Task.Run(() => { throw null; });
Task task2 = Task.Run(() => { throw null; });
Task all = Task.WhanAll(task1, task2);
try { await all }
catch
{
    Console.WriteLine(all.Exception.InnerExceptions.Count); // 2
}
```
+ 对一组Task<TResult>调用WhenAll会返回Task<TResult[]>，也就是所有Task的组合结果。
+ 如果进行await，那么就会得到TResult[]：
```
Task<int> task1 = Task.Run(() => 1);
Task<int> task2 = Task.Run(() => 2);
int[] results = await Task.WhenAll(task1, task2); // { 1, 2 }
```

## 实例
```
async Task<int> GetTotalSize(string[] uris)
{
    IEnumerable<byte[]> downloadTasks = uris.Select(uri => new WebClient().DownloadDataTaskAsync(uri));
    byte[][] contents = await Task.WhenAll(downloadTasks);
    return contents.Sum(c => c.Length);
}
```
+ 上面的代码优化一下如下：
```
async Task<int> GetTotalSize(string[] uris)
{
    IEnumerable<Task<int>> downloadTasks = uris.Select(async uri => (await new WebClient().DownloadDataTaskAsync(uri)).Length);
    int[] contentLengths = await Task.WhenAll(downloadTasks);
    return contentLengths.Sum();
}
```

## 自定义Task组合器
+ 可以编写自定义的Task组合器。最简单的组合器接收一个Task，看下例：
```
async static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, Timespan timeout)
{
    Task winner = await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);
    if (winner != task) throw new TimeoutException();
    return await task.ConfigureAwait(false);  // Unwrap result/re-throw
}
```
+ 这就是为等待的Task添加了超时的功能。
+ 因为这很可能是一个库方法，无需与外界共享状态，所以在await时我们使用了ConfigureAwait(false)来避免弹回到UI的同步上下文。
+ 通过在Task完成时取消Task.Delay我们可以改进上例的效率(避免了计时器的小开销)：
```
async static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
{
    var cancelSource = new CancellationTokenSource();
    var delay = Task.Delay(timeout, cancelSource.Token);
    Task winner = await Task.WhenAny(task, delay).ConfigureAwait(false);
    if (winner == task)
        cancelSource.Cancel();
    else
        throw new TimeoutException();
    return await task.ConfigureAwait(false);  // Unwrap result/re-throw
}
```

## 自定义Task组合器 通过CancellationToken放弃Task
```
static Task<TResult> WithCancellation<TResult>(this Task<TResult> task, CancellationToken cancelToken)
{
    var tcs = new TaskCompletionSource<TResult>();
    var reg = cancelToken.Register(() => tcs.TrySetCanceled());
    task.ContinueWith(ant =>
    {
        reg.Dispose();
        if (ant.IsCanceled)
            tcs.TrySetCanceled();
        else if (ant.IsFaulted)
            tcs.TrySetException(ant.Exception.InnerException);
        else
            tcs.TrySetResult(ant.Result);
    });
    return tcs.Task;
}
```
-
+ 这个组合器功能类似WhenAll，如果一个Task出错，那么其余的Task也立即出错：
```
static Task<TResult> WhenAllOrError<TResult>(params Task<TResult> tasks)
{
    var killJoy = new TaskCompletionSource<TResult[]>();
    foreach (var task in tasks)
        task.ContinueWith(ant =>
        {
            if (ant.IsCanceled)
                killJoy.TrySetCanceled();
            else if (ant.IsFaulted)
                killJoy.TrySetException(ant.Exception.InnerException);
        });
    return await await Task.WhenAny(killJoy.Task, Task.WhenAll(tasks)).ConfigureAwait(false);
}
```
  + 这里面TaskCompletionSource的任务就是当任意一个Task出错时，结束工作。所以我们没调用SetResult方法，只调用了它的TrySetCanceled和TrySetException方法。在这里ContinueWith要比GetAwaiter().OnCompleted更方便，因为我们不访问Task的result，并且此刻不想弹回到UI线程。