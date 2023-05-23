package org.example;

import java.io.*;
import jakarta.servlet.*;
import jakarta.servlet.http.*;
import jakarta.servlet.annotation.WebServlet;
import java.util.*;

@WebServlet("/task")
public class TaskServlet extends HttpServlet {
    @Override
    public void init(ServletConfig config) throws ServletException {
        super.init(config);
    }

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        resp.setContentType("text/html");

        try (var writer = resp.getWriter()) {
            writer.write("<h2>Запрос:</h2>");
            Iterator<String> headerNames = req.getHeaderNames().asIterator();
            while (headerNames.hasNext()) {
                String headerName = headerNames.next();
                String headerValue = req.getHeader(headerName);
                writer.write("<p>" + headerName + ": " + headerValue + "</p>");
            }
        }
    }

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        resp.setContentType("text/html");

        try (var reader = req.getReader(); var writer = resp.getWriter()) {
            var lines = reader.lines();
            lines.forEach(System.out::println);
            writer.write("<p>HIIIII</p>");
        }
    }

    @Override
    public void destroy() {
        super.destroy();
    }
}
