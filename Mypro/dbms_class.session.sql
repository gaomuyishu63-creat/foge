/*課題1*/
SHOW tables;
DESCRIBE attendance;
DESCRIBE courses;
DESCRIBE enrollment;
DESCRIBE students;

/*課題2*/
SELECT *
FROM students
LIMIT 10;

/*課題3*/
SELECT a.student_id
FROM attendance AS a
LEFT JOIN students AS s
  ON a.student_id = s.student_id
WHERE s.student_id IS NULL
GROUP BY a.student_id
ORDER BY a.student_id;

/*課題4*/
SELECT a.course_id
FROM attendance AS a
LEFT JOIN courses AS c
  ON a.course_id = c.course_id
WHERE c.course_id IS NULL
GROUP BY a.course_id
ORDER BY a.course_id;

/*課題5*/
SELECT e.course_id
FROM enrollment AS e
LEFT JOIN courses AS c
  ON e.course_id = c.course_id
WHERE c.course_id IS NULL
GROUP BY e.course_id
ORDER BY e.course_id;

/*課題6*/
SELECT
  a.student_id,
  COUNT(*) AS attend_count
FROM attendance AS a
WHERE a.status = '出席'
GROUP BY a.student_id
ORDER BY a.student_id;


/*課題7*/
SELECT
  a.student_id,
  COUNT(*) AS attend_count
FROM attendance AS a
WHERE a.status = '出席'
GROUP BY a.student_id
ORDER BY attend_count DESC, a.student_id ASC;


/*課題8*/
SELECT
  a.student_id,
  COUNT(*) AS attend_count
FROM attendance AS a
WHERE a.status = '出席'
GROUP BY a.student_id
HAVING COUNT(*) >= 3;

/*課題9*/
SELECT
  a.student_id,
  COUNT(*) AS attend_count,
  CASE
    WHEN COUNT(*) >= 3 THEN '高出席'
    WHEN COUNT(*) >= 2 THEN '中出席'
    ELSE '低出席'
  END AS 出席区分
FROM attendance AS a
WHERE a.status = '出席'
GROUP BY a.student_id
ORDER BY attend_count DESC, a.student_id ASC;

/*課題10*/
SELECT
  student_id,
  CONCAT(name, 'さん') AS name_san
FROM students
ORDER BY student_id;

/*ボーナス1*/
SELECT student_id, course_id, COUNT(*) date, status
FROM attendance
WHERE LOWER(TRIM(status)) NOT IN ('出席', '欠席');

/* テスト：誤データ(山田誤・吉田誤)をまとめて検索 */
SELECT *
FROM students
WHERE name IN ('山田誤', '吉田誤');

/*ボーナス*/
SELECT student_id, name, dept_code, enroll_year
FROM students
WHERE CHAR_LENGTH(student_id) <> 7
ORDER BY student_id;


