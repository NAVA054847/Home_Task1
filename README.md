# מערכת חישוב זכאות להחזר כספי 

מערכת לניהול בקשות החזר מס: אזרחים יוצרים בקשות, והפקיד מחשב זכאות, מאשר או דוחה.

---





## דרישות מקדימות

- **.NET 8 SDK** – להרצת ה-API
- **SQL Server** – מסד הנתונים
- **Node.js 18+** ו-**npm** – להרצת ממשק הלקוח (React)

---

## התקנה והרצה

### 1. מסד הנתונים

<div dir="rtl">

1. פתחו את **SQL Server Management Studio** והתחברו.

2. הרצו את סקריפט יצירת המסד והטבלאות:
   - **קובץ:** `RefundSystem/Scripts/CreateDatabase.sql`
   - הקובץ יוצר את המסד `RefundSystemDB` והטבלאות.

3. הרצו את סקריפט **נתוני הדמה** (כדי שאפשר יהיה לבדוק את המערכת עם נתונים):
   - **קובץ:** `RefundSystem/Scripts/SeedDemoData.sql`
   - מזין אזרחים, פקידים, הכנסות חודשיות, בקשות ותקציב. ניתן להריץ גם אם כבר הרצתם את CreateDatabase (הסקריפט לא יכפיל נתונים קיימים).

4. הרצו את סקריפטי הפרוצדורות (לפי הסדר):
   - `RefundSystem/Scripts/StoredProcedures/CreateRefundRequest.sql`
   - `RefundSystem/Scripts/StoredProcedures/CalculateRefund.sql`
   - `RefundSystem/Scripts/StoredProcedures/GetPendingRequestForClerk.sql`
   - `RefundSystem/Scripts/StoredProcedures/GetRequestDetailsForClerk.sql`
   - `RefundSystem/Scripts/StoredProcedures/GetCitizenRequestsHistory.sql`
   - `RefundSystem/Scripts/StoredProcedures/GetCitizenRequestView.sql`
   - `RefundSystem/Scripts/StoredProcedures/ApproveOrRejectRefundRequest.sql`

### 2. עדכון מחרוזת החיבור

ב־`RefundSystem/RefundSystem.API/appsettings.json` עדכנו את `ConnectionStrings:DefaultConnection` לפי שרת ה-SQL שלכם, למשל:

```json
"DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=RefundSystemDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 3. הרצת ה-API (Backend)

```bash
cd RefundSystem
dotnet run --project RefundSystem.API
```



### 4. הרצת ממשק הלקוח (Frontend)

```bash
cd front_refundsystem
npm install
npm start
```

האפליקציה תיפתח בדפדפן. 

---

## משתמשים לניסיון (נתוני הדמה)

לאחר הרצת `SeedDemoData.sql` (או עם נתוני ההתחלה ב-CreateDatabase):

- **אזרחים:** ת.ז. `123446789` (יוסי כהן), `987654321` (דנה לוי)
- **פקידים:** ת.ז. `999999999` (פקידת ניסוי), `888888888` (פקיד בדיקה)

במסך ההתחברות מזינים תעודת זהות – המערכת מפנה אוטומטית למסך האזרח או למסך הפקיד לפי סוג המשתמש.

---

## מבנה הפרויקט

| תיקייה / קובץ | תיאור |
|---------------|--------|
| `RefundSystem/` | פתרון .NET (API, Core, Service, Data) |
| `RefundSystem/RefundSystem.API` | Web API – נקודות הקצה |
| `RefundSystem/RefundSystem.Core` | ישויות, ממשקים, Read Models |
| `RefundSystem/RefundSystem.Service` | לוגיקה עסקית |
| `RefundSystem/RefundSystem.Data` | גישה ל-DB, Repository, הרצת פרוצדורות |
| `RefundSystem/Scripts/` | סקריפט SQL – יצירת DB ופרוצדורות |
| `RefundSystem/Scripts/CreateDatabase.sql` | יצירת המסד והטבלאות |
| `RefundSystem/Scripts/SeedDemoData.sql` | הזנת נתוני דמה (אזרחים, פקידים, הכנסות, בקשות, תקציב) |
| `RefundSystem/Scripts/StoredProcedures/` | קובץ SQL נפרד לכל פרוצדורה |
| `front_refundsystem/` | ממשק React (אזרח / פקיד) |

---






## פיתוחים נוספים

- **מסך התחברות לפי תעודת זהות** – מזינים ת.ז. בלבד; המערכת בודקת אם זה פקיד או אזרח ומפנה בהתאם: תז של פקיד → מסך הפקיד (רשימת בקשות ממתינות), תז של אזרח → מסך האזרח (היסטוריית בקשות).

---



