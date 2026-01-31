const BASE_URL = "http://localhost:52705/api";

/* ================= TOKEN HELPER ================= */
const getToken = () => {
  return sessionStorage.getItem("token");
};

const authHeader = () => {
  const token = getToken();
  return token
    ? { Authorization: `Bearer ${token}` }
    : {};
};

// -------- AUTH --------
export const loginUser = async (data) => {
  const res = await fetch(`${BASE_URL}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data)
  });

  const result = await res.json();

  // 🔑 STORE JWT TOKEN
  if (res.ok && result.token) {
    sessionStorage.setItem("token", result.token);
    sessionStorage.setItem("user", JSON.stringify(result.user));
  }

  return result;
};


export const googleLogin = async (user) => {
  const res = await fetch(`${BASE_URL}/auth/google`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(user)
  });

  const text = await res.text();
  const result = text ? JSON.parse(text) : null;

  // 🔑 STORE JWT TOKEN
  if (res.ok && result?.token) {
    sessionStorage.setItem("token", result.token);
    sessionStorage.setItem("user", JSON.stringify(result.user));
  } else {
    throw new Error("Google login failed");
  }

  return result;
};

// -------- MODULES --------
export const getModules = async () => {
  const res = await fetch(`${BASE_URL}/modules`, {
    headers: {
      ...authHeader()
    }
  });
  return res.json();
};

// -------- QUIZ --------
export const getQuestions = async (moduleId) => {
  const res = await fetch(`${BASE_URL}/quiz/${moduleId}`, {
    headers: {
      ...authHeader()
    }
  });
  return res.json();
};

// -------- RESULT --------
export async function saveResult(result) {
  const res = await fetch(`${BASE_URL}/result`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...authHeader()
    },
    body: JSON.stringify({
      userId: result.userId,
      moduleId: result.moduleId,
      score: result.score,
      attempted: result.attempted,
      unattempted: result.unattempted,
      testType: result.testType,

      // ✅ ALWAYS SEND mockNo
      mockNo: result.testType === "Mock"
        ? result.mockNo
        : null
    })
  });

  if (!res.ok) {
    const error = await res.text();
    throw new Error(error);
  }
}

// -------- RESULT STATS --------
export const getAttemptSummary = async (userId) => {
  const res = await fetch(
    `${BASE_URL}/result/attempts/${userId}`,
    {
      headers: {
        ...authHeader()
      }
    }
  );
  return res.json();
};

export async function getLatestResultStats(userId) {
  const res = await fetch(
    `${BASE_URL}/result/latest/${userId}`,
    {
      headers: {
        ...authHeader()
      }
    }
  );

  if (!res.ok) {
    return {
      moduleName: "N/A",
      score: 0,
      attempted: 0,
      unattempted: 0,
      totalTests: 0,
      practiceTests: 0,
      mockTests: 0,
      bestScore: 0
    };
  }

  const contentType = res.headers.get("content-type");

  if (!contentType || !contentType.includes("application/json")) {
    return {
      moduleName: "N/A",
      score: 0,
      attempted: 0,
      unattempted: 0,
      totalTests: 0,
      practiceTests: 0,
      mockTests: 0,
      bestScore: 0
    };
  }

  return await res.json();
};

// -------- MOCK --------
export const getMockQuestions = async (moduleId, mockNumber) => {
  const res = await fetch(
    `${BASE_URL}/mock/${moduleId}/${mockNumber}`,
    {
      headers: {
        ...authHeader()
      }
    }
  );

  if (!res.ok) throw new Error("Failed to load mock test");
  return res.json();
};

export const checkMockAttempt = (userId, moduleId, mockNo) => {
  return fetch(
    `${BASE_URL}/result/check-mock?userId=${userId}&moduleId=${moduleId}&mockNo=${mockNo}`,
    {
      headers: {
        ...authHeader()
      }
    }
  ).then(res => res.json());
};


