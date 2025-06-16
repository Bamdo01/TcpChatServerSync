# C# TCP 동기식 채팅 서버

.NET 환경에서 C#으로 구현한 TCP 기반의 동기식 멀티 채팅 서버입니다. 저수준 소켓 프로그래밍, 동시성 제어, 데이터베이스 연동 및 기본적인 보안 원칙을 학습하고 적용하는 것을 목표로 제작되었습니다.

## 📑 주요 기능

* **회원가입:** ID, 암호, 전화번호, 성별, 생년월일 정보를 받아 사용자를 등록합니다.
* **로그인:** ID와 암호를 검증하며, 성공 시 채팅 서비스에 접속하고 클라이언트 목록에 추가됩니다.
* **실시간 멀티 채팅:** 로그인된 사용자는 자신을 제외한 다른 모든 접속자에게 메시지를 보내고 받을 수 있습니다 (Broadcast).
* **서버 관리:** 서버 콘솔에서 `online` 명령어로 현재 접속자 목록을 확인하거나 `exit`으로 서버를 종료할 수 있습니다.

## 🛠️ 기술 스택 및 아키텍처

* **언어 및 프레임워크:** C# (.NET)
* **데이터베이스:** MySQL (with `MySqlConnector`)
* **핵심 기술:** TCP/IP 소켓 프로그래밍 (`TcpListener`, `TcpClient`), 멀티스레딩 (`Thread`)

### 아키텍처 다이어그램

[여기에 아키텍처 다이어그램 이미지를 삽입하세요]

### 주요 설계 특징
* **계층형 아키텍처:** `Core`, `Network`, `Database`, `Managers` 등 기능별로 프로젝트를 분리하여 코드의 응집도와 재사용성을 높였습니다.
* **클라이언트당 스레드 모델:** 각 클라이언트의 연결을 별도의 스레드에서 처리하여 동시 요청에 대응합니다.
* **스레드 안전성:** 공유 자원인 접속자 목록(`connectedClients`)에 `lock`을 적용하여 여러 스레드에서의 동시 접근으로 인한 문제를 방지합니다.
* **보안:**
    * **비밀번호 암호화:** `SHA256` 해시와 `Salt`를 이용해 단방향 암호화를 구현하여 사용자 정보를 보호합니다.
    * **SQL Injection 방지:** 모든 데이터베이스 쿼리에 파라미터화된 쿼리를 사용하여 외부 공격으로부터 안전하게 보호합니다.
* **커스텀 바이너리 프로토콜:** 바이트 단위의 자체 프로토콜을 `PacketBuilder`와 `PacketParser`로 정의하여 통신합니다.

## 💾 데이터베이스 스키마
`DbManager.InitializeDatabase()` 메서드에 의해 서버 시작 시 아래 테이블들이 자동으로 생성됩니다.
```sql
-- 사용자 정보 테이블
CREATE TABLE IF NOT EXISTS users (
    id INT PRIMARY KEY AUTO_INCREMENT,
    userid VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    salt VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    gender VARCHAR(10) NOT NULL,
    birthdate DATE NOT NULL
);

-- 채팅 메시지 로그 테이블
CREATE TABLE IF NOT EXISTS chat_messages (
    id INT PRIMARY KEY AUTO_INCREMENT,
    sender_id VARCHAR(50) NOT NULL,
    message TEXT NOT NULL,
    sent_at DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

## 🌐 통신 프로토콜

서버와 클라이언트는 다음 형식의 바이너리 패킷으로 통신합니다. 문자열은 [2바이트 길이 정보] + [UTF-8 바이트 배열]로 직렬화됩니다

## 🚀 실행 방법

1. Prerequisites: .NET SDK, MySQL Server 설치
2. MySQL에 `NEXTCHAT` 데이터베이스를 생성합니다.
3. `Database/DbManager.cs` 파일의 연결 문자열(`connStr`)을 자신의 DB 환경에 맞게 수정합니다.
4. 프로젝트를 빌드하고 실행합니다.
5. 서버 콘솔에 `[서버] 시작됨 - 포트: 9000` 메시지가 표시되면 정상적으로 실행된 것입니다.
6. 
## ✨ 향후 개선 과제

* ** 비동기 I/O 모델로 전환
