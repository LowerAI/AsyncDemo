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