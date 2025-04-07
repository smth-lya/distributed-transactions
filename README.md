**Ru**: Распределенные транзакции, паттерн **Saga** (хореография и оркестрация), **2PC**, 3PC

**En**: Distributed transactions, **Saga** pattern (choreography and orchestration), **2PC**, 3PC

- Презентация (Google Slides) -> [Ссылка **Тык**](https://docs.google.com/presentation/d/1n31xz_v1AkadDUAohH4xvRPKAMS7G-vzz0aAuqUTNqU/edit?usp=sharing)
- PDF вариант (в случае если ссылка не сработала) -> *Тык*



### Design Document: Distributed Transactions (Saga, 2-Phase Commit) для ASP.NET/C#  
**Программа минимум:** Базовая реализация распределённых транзакций с поддержкой Saga (оркестрация) и 2PC.  

---

### 1. Требования  
#### 1.1 Функциональные требования  
- **Минимальные:**  
  - 4 сервиса: Gateway (ASP.NET Core) + 3 микросервиса (Order, Payment, Inventory).  
  - Синхронное взаимодействие через HTTP/REST (ASP.NET Core Web API).  
  - Saga Orchestration с центральным координатором (на C#).  
  - 2-Phase Commit для транзакций внутри одного сервиса.  
  - Локальное хранение состояний: In-Memory (ConcurrentDictionary) + логи (Serilog).  

- **Максимальные (расширения):**  
  - 5+ сервисов с репликацией (Docker/Kubernetes) и балансировкой (Nginx).  
  - Асинхронная коммуникация через RabbitMQ/Kafka (MassTransit).  
  - Saga Choreography через события (Event-Driven).  
  - Горячая конфигурация через Consul или Azure App Configuration.  
  - Хранилище состояний: PostgreSQL (Entity Framework), Redis (StackExchange.Redis).  
  - Мониторинг: Prometheus.NET + Grafana, логи в ELK-стеке.  

#### 1.2 Нефункциональные требования  
- **Консистентность:**  
  - Для Saga: Eventual consistency (через компенсации).  
  - Для 2PC: Strong consistency (транзакции БД через System.Transactions).  
- **Отказоустойчивость:** Повторные попытки (Polly), Health Checks (ASP.NET Core Health Checks).  

---

### 2. Архитектура системы  
#### 2.1 Компоненты  
1. **Gateway (ASP.NET Core):**  
   - Маршрутизация запросов, аутентификация (JWT), агрегация данных.  
   - Документация API: Swagger (Swashbuckle).  

2. **Сервисы:**  
   - **Order Service:** Управление заказами, запуск Saga.  
   - **Payment Service:** Обработка платежей, интеграция с 2PC.  
   - **Inventory Service:** Управление остатками, компенсации.  
   - **Saga Orchestrator (отдельный сервис):** Координация шагов транзакции.  

3. **Инфраструктура:**  
   - **Синхронное взаимодействие:** ASP.NET Core Web API + HttpClient.  
   - **Асинхронное взаимодействие:** RabbitMQ (MassTransit) для Saga Choreography.  
   - **Базы данных:**  
     - Минимум: SQLite (In-Memory для тестов).  
     - Максимум: PostgreSQL + Redis для кэша состояний Saga.  

---

### 3. Реализация  
#### 3.1 Минимальная версия  
- **Saga Orchestration:**  
  - Координатор на C# с использованием `BackgroundService`.  
  - Пример кода:  
    ```csharp  
    public class OrderSagaOrchestrator : BackgroundService  
    {  
        protected override async Task ExecuteAsync(CancellationToken token)  
        {  
            try  
            {  
                await _orderService.ReserveOrder();  
                await _paymentService.ProcessPayment();  
            }  
            catch  
            {  
                await _paymentService.CompensatePayment();  
                await _orderService.CancelOrder();  
            }  
        }  
    }  
    ```  

- **2PC:**  
  - Использование `TransactionScope` для локальных транзакций:  
    ```csharp  
    using (var scope = new TransactionScope())  
    {  
        _db1.SaveChanges();  
        _db2.SaveChanges();  
        scope.Complete();  
    }  
    ```  

#### 3.2 Расширения  
- **Saga Choreography:**  
  - События через MassTransit + RabbitMQ:  
    ```csharp  
    public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>  
    {  
        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)  
        {  
            await _inventoryService.UpdateStock(context.Message.OrderId);  
        }  
    }  
    ```  

- **Динамическая конфигурация:**  
  - Интеграция с Consul:  
    ```csharp  
    services.AddConsulConfig(configuration);  
    ```  

---

### 4. Инфраструктура и DevOps  
- **Репликация:**  
  - Минимум: Docker Compose для локальных реплик.  
  - Максимум: Kubernetes с Helm-чартами.  
- **Балансировка:**  
  - Nginx как reverse proxy для Gateway.  
- **Метрики:**  
  - Prometheus.NET + Grafana для сбора метрик времени ответа и ошибок.  
  ```csharp  
  app.UseMetricServer(); // Prometheus  
  ```  

---

### 5. Сравнение минимум/максимум  
| **Компонент**       | **Минимум**                     | **Максимум**                          |  
|---------------------|---------------------------------|---------------------------------------|  
| **Сервисы**         | 4 (без репликации)             | 5+ с балансировкой и репликацией      |  
| **Взаимодействие**  | HTTP (синхронное)              | RabbitMQ/Kafka (асинхронное)          |  
| **Saga**            | Оркестрация                    | Оркестрация + Хореография             |  
| **Конфигурация**    | appsettings.json               | Consul / Azure App Configuration      |  
| **Хранилище**       | In-Memory + SQLite             | PostgreSQL + Redis                    |  
| **Логирование**     | Serilog + файлы                | ELK-стек (Elasticsearch, Logstash)    |  

---

### 6. Варианты расширения  
1. **Интеграция с облаком:**  
   - Azure Service Bus для асинхронной коммуникации.  
   - Azure SQL Database для хранилища.  
2. **Оптимизация Saga:**  
   - Использование Azure Durable Functions для долгих транзакций.  
3. **Безопасность:**  
   - OAuth2/OIDC через IdentityServer4.  
4. **Тестирование:**  
   - Интеграционные тесты с Testcontainers.  

---

### 7. Заключение  
**Программа минимум:**  
- Gateway + 3 сервиса на ASP.NET Core.  
- Saga Orchestration + 2PC с TransactionScope.  
- Локальное логирование и In-Memory хранилище.  

**Расширения:**  
- Асинхронные Saga через MassTransit, интеграция с ELK/Prometheus, Kubernetes.  

Документ учитывает особенности экосистемы .NET и обеспечивает гибкость для масштабирования.
