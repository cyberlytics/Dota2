FROM jupyter/scipy-notebook:python-3.9.2

RUN pip install --no-cache-dir jupyter_kernel_gateway

COPY . /home/bdcc/

# Configure container startup, letting the user pass command line args through
# easily without disrupting the use of tini or the kernelgateway entry point by adding
# those commands with CMD for example by docker-compose.yml
ENTRYPOINT ["tini", "--"]
