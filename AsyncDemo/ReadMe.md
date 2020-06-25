# 添加超时
+ 调用`Join`的时候，可以设置一个超时，用毫秒或者`TimeSpan`都可以。
  + 如果返回`true`，那就是线程结束了；如果超时了，就返回`false`。
+ `Thread.Sleep()`方法会暂停当前的线程，并等待一段时间。

+ 注意：
 + `Thread.Sleep(0)`这样调用会导致线程立即放弃本身当前的时间片，自动将CPU移交给其他线程。
 + `Thread.Yield()`做同样的事情，但是它只会把执行交给同一处理器上的其他线程。
 + 当前`Sleep`或`Join`的时候，线程处于阻塞的状态。

# 阻塞
+ 如果线程的执行由于某种原因导致暂定，那么就认为该线程被阻塞了。
  + 例如`Sleep`或者通过`Join`等待其他线程结束。
+ 被阻塞的线程会立即将其处理器的时间片生成给其它线程，从此就不在消耗处理器时间，直到满足其阻塞条件为止。
+ 可以通过`ThreadState`这个属性来判断线程是否处于被阻塞的状态：  
  `bool blocked = (someThread.ThreadState & ThreadState.waitSleepJoin) != 0;`

# ThreadState
+ `ThreadState`是一个`flags enum`，通过按位的形式，可以合并数据的选项。

# 解除阻塞 Unlocking
+ 当遇到下列四种情况的时候，就会解除阻塞：
  + 阻塞条件被满足
  + 操作超时(如果设置超时的话)
  + 通过`Thread.Interrupt()`进行打断
  + 通过`Thread.Abort()`进行中止

# I/O-bound vs Compute-bound(或CPU-bound)
+ 一个花费大部分时间等待某事发生的操作称为I/O-bound
  + I/O绑定操作通常涉及输入输出，但这不是硬性要求：`Thread.Sleep()`也被视为`I/O-bound`
+ 相反，一个花费大部分时间执行CPU密集型工作的操作称为`Compute-bound`

# 阻塞 vs 忙等待(自旋) Blocking vs Spinning
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

# 本地 vs 共享的状态 Local vs Shared State
### Local 本地的独立
+ CLR为每个线程分配自己的内存栈(Stack)，以便使本地变量保持独立。

### Shared 共享
+ 如果多个线程都引用到同一个对象的实例，那么他们就共享了数据。
+ 被Lambda表达式或匿名委托所捕获的本地变量，会被编译器转化为字段(field)，所以也会被共享。
+ 静态字段(field)也会在线程间共享数据。

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

# 向线程传递数据
+ 如果你想往线程的启动方法里传递参数，最简单的方式是使用lambda表达式，在里面使用参数调用方法。(例子lambda)
+ 甚至可以把整个逻辑都放在lambda里面。(例子multi-lambda)
+ 在C#3.0之前，没有lamdba表达式。可以使用Thread的Start方法传递参数。(例子old-school)
+ Thread的重载构造函数可以接受下列两个委托之一作为参数：
  ···
  public delegate void ThreadStart();
  public delegate void ParameterizedThreadStart(object obj);
  ···

# Lamdba 表达式与被捕获的变量
+ 使用Lamdba表达式可以很简单的给Thread传递参数。但是线程开始后。可能会不小心修改了被捕获的变量，这要多加注意。(例子captured)

# 异常处理
+ 创建线程时在作用范围内的try/catch/finally块，在线程开始执行后就与线程无关了。
+ 在WPF、WinForm里，可以订阅全局一场处理事件：
  + Application.DispatcherUnhandledException
  + Application.ThreadException
  + 在通过消息循环调用的程序的任何部分发生未处理的异常(这相当于应用程序处于活动状态时在主线程上运行的所有代码)后，将触发这些异常。
  + 但是非 UI 线程上的未处理异常，并不会触发它。
+ 而任何线程有任何未处理的异常都会触发
  AppDomain.CurrentDomain.UnhandledException

# Foreground vs Background Threads 前台和后台线程
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

# 线程优先级
+ 线程的优先级(Thread的Priority属性)决定了相对于操作系统中其它活跃线程所占的执行时间。
+ 优先级分为：
  + enum ThreadPriority { Lowest,BelowNormal,Normal,AboveNormal,Highest }

# 提升线程优先级
+ 提升线程优先级的时候需要特别注意，因为它可能“饿死”其它线程。
+ 如果想让某线程(Thread)的优先级比其他进程(Procee)中的线程(Thread)高，那么就必须提升进程(Process)的优先级
  + 使用System.Diagnostics下的Process类。
  ···
    using (Process p = Process.GetCurrentProcess())
        p.PriorityClass = ProcessPriorityClass.High;
  ···
+ 这可以很好地适用于只做少量工作且需要较低延迟的非UI进程。
+ 对于需要大量计算的应用程序(尤其是有UI的应用程序)，提高进程优先级可能会使其他进程饿死，从而降低整个计算机的速度。

# 信号 Signaling
+ 有时，你需要让某线程一直处于等待的状态，直至接收到其它线程发来的通知。这就叫做singaling(发送信号)。
+ 最简单的信号结构就是ManualResetEvent。
  + 调用它上面的WaitOne方法会阻塞当前的线程，直到另一个线程通过调用Set方法来开启信号。
  + (例子signaling)
    + 调用完Set之后，信号会处于“打开”的状态。可以通过调用Reset方法将其再次关闭。
    

# 富客户端应用程序的线程
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
+ Invoke执行相同的操作，但随后会及进行阻塞，直到UI线程读取并处理消息。
  + 因此，Inovke允许您从方法中获取返回值。
  + 如果不需要返回值，BeginInovke/RunAsync更可取，因为他们不会阻塞调用方，也不会引入思索的可能性

# Synchronization Contexts 同步上下文
+ 在System.ComponmentModel下有一个抽象类：SynchronizationContext，它使得Thread Marshaling得到泛化。
+ 针对移动、桌面(WPF，UWP，WinForm)等富客户端应用的API，他们都定义和实例化了SynchronizationContext的子类
  + 可以通过静态属性SynchronizationContext.Current来获得(当运行在UI线程时)
  + 捕获该属性让你可以在稍后的时候从worker线程向UI线程发送数据(例子)
  + 调用Post就相当于调用Dispatch或Control上面的BeginInovke方法
  + 还有一个Send方法，它等价于Inovke方法

# 线程池 Thread Pool
+ 当开始一个线程的时候，将花费几百微妙来组织似以下的内容：
  + 一个新的局部变量栈
+ 线程池就可以节省这种开销
  + 通过预先创建一个可循环使用线程的池来减少这一开销。
+ 线程池对于高效的并行编程和细粒度开发是必不可少的
+ 它允许不被线程启动的开销淹没的情况下运行短期操作

# 使用线程池线程需要注意的几点
+ 不可以设置池线程的Name
+ 池线程都是后台线程
+ 阻塞池线程可使性能降低

+ 你可以自由的更改池线程的优先级
  + 当它释放回池的时候优先级将还原为正常状态
+ 可以通过Thread.CurrentThread.IsThreadPoolThread属性来判断是否

# 进入线程池
+ 最简单的、显示的在线程池运行代码的方式就是使用Task.Run
```
// Task is in System.Threading.Tasks
Task.Run(() => Console.WriteLine("Hello from the thread pool"));
```

# 谁使用了线程池
+ WCF、Remoting、ASP.NET、ASMX Web Services应用服务器
+ System.Timers.Timer、System.Threading.Timer
+ 并行编程结构
+ BackgroundWorker类(现在很多余)
+ 异步委托(现在很多余)

# 线程池中的整洁 CLR的策略
+ CLR通过对任务排队并对其启动进行节流限制来避免线程池中的超额订阅。
+ 它首先运行尽可能多的并发任务(只要还有CPU核)，然后通过爬山算法调整并发级别，并在特定方向上不断调整工作负载。
  + 如果吞吐量提高，它将继续朝同一方向(否则将反转)。
+ 这确保它始终追随最佳性能曲线，即使面对计算机上竞争的进程活动时也如此
+ 如果下面两点能够满足，那么CLR的策略将发挥出最佳效果：
  + 工作项大多是短时间运行的(<250毫秒，或者理想情况下<100毫秒)，因此CLR有很多机会进行测量和调整。
  + 大部分时间都被阻塞的工作项不会主宰线程池

# Thread的问题
+ 线程(Thread)是用来创建并发(concurrency)的一种低级别工具，它有一些限制，尤其是：
  + 虽然开始线程的时候可以方便的传入数据，但是当Join的时候，很难从线程获得返回值。
    + 可能需要设置一些共享字段。
    + 如果操作抛出异常，捕获和传播该异常都很麻烦。
  + 无法告诉线程在结束时开始做另外的工作，你必须进行Join操作(在进程中阻塞当前的线程)
+ 很难使用较小的并发(concurrent)来组建大型的并发
+ 导致了对手动同步的更大依赖以及随之而来的问题

# Task Class
+ Task类可以很好的解决上述问题
+ Task是一个相对高级的抽象：它代表了一个并发操作(concurrent)
  + 该操作可能由Thread支持，或不由Thread支持
+ Task可以使用线程池来减少启动延迟
+ 使用TaskCompletionSource，Task可以利用回调的方式吗，在等待I/O绑定操作时完全避免线程。

# 开始一个Task Task.Run
+ Task类在System.Threading.Tasks命名空间下。
+ 开始一个Task最简单的办法就是使用Task.Run(.NET 4.5, 4.0的时候是Task.Factory.StartNew)这个静态方法：
  + 传入一个Action委托即可(例子task)
+ Task默认使用线程池，也就是后台线程：
  + 当主线程结束时，你创建的所有tasks都会结束。(例子task)
+ Task.Run返回一个Task对象，可以使用它来监视其过程
  + 在Task.Run之后，我们没有调用Start，因为该方法创建的时“热”任务(hot task)
    + 可以通过Task的构造函数创建“冷”任务(cold task)，但是很少这样做
  + 可以通过Task的Status属性来跟踪task的执行状态。

# Wait 等待
+ 调用task.Wait方法会进行阻塞直到操作完成
  + 相当于调用thread上的Join方法
  + (例子wait)
+ Wait也可以让你指定一个超时时间和一个取消令牌提前结束等待。

# Long-runnning tasks 长时间运行的任务
+ 默认情况下，CLR在线程池中运行Task，这非常适合短时间运行的Compute-Bound类工作。
+ 针对长时间运行的任务或者阻塞操作(例如前面的例子)，你可以不采用线程池(例子longRunning)
+ 如果同时运行多个long-running tasks(尤其时其中有处于阻塞状态的)，那么性能会受很大影响，这时有比TaskCreationOptions.LongRunning更好的办法：
  + 如果任务时IO-Bounds，TaskCompletionSource和异步函数可以让你用回调(Coninuations)代替线程来实现并发。
  + 如果任务时Compute-Bound，生产者/消费者队列允许你对任务的并发性进行限流，避免把其他线程和进程饿死。